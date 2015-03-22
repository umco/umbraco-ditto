namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// The lazy proxy factory for creating proxies representing the given type with any virtual properties
    /// or methods lazily invoked.
    /// </summary>
    public class LazyProxyFactory
    {
        /// <summary>
        /// The base constructor.
        /// </summary>
        private static readonly ConstructorInfo BaseConstructor = typeof(object).GetConstructor(new Type[0]);

        /// <summary>
        /// The get type from handle.
        /// </summary>
        private static readonly MethodInfo GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

        /// <summary>
        /// The serialization info get value method.
        /// </summary>
        private static readonly MethodInfo GetValue = typeof(SerializationInfo).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(Type) }, null);

        /// <summary>
        /// The serialization info get type method.
        /// </summary>
        private static readonly MethodInfo SetType = typeof(SerializationInfo).GetMethod("SetType", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(Type) }, null);

        /// <summary>
        /// The add value.
        /// </summary>
        private static readonly MethodInfo AddValue = typeof(SerializationInfo).GetMethod("AddValue", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(object) }, null);

        /// <summary>
        /// The type map lock for locking round the type dictionary.
        /// </summary>
        private static readonly ReaderWriterLockSlim TypeMapLock = new ReaderWriterLockSlim();

        /// <summary>
        /// The proxy method builder.
        /// </summary>
        private readonly ProxyMethodBuilder proxyMethodBuilder;

        /// <summary>
        /// The proxy cache.
        /// </summary>
        private readonly ProxyCache proxyCache = new ProxyCache();

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyProxyFactory"/> class.
        /// </summary>
        public LazyProxyFactory()
        {
            this.proxyMethodBuilder = new ProxyMethodBuilder();
        }

        /// <summary>
        /// Creates a proxy representing the given type with any virtual properties
        /// or methods lazily invoked.
        /// </summary>
        /// <param name="instanceType">
        /// The instance type.
        /// </param>
        /// <param name="interceptor">
        /// The lazy interceptor.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <returns>
        /// The proxy <see cref="object"/>.
        /// </returns>
        public object CreateLazyProxy(Type instanceType, LazyInterceptor interceptor, params Type[] baseInterfaces)
        {
            Type proxyType = this.CreateProxyType(instanceType, baseInterfaces);
            object result = Activator.CreateInstance(proxyType);
            IProxy proxy = (IProxy)result;
            proxy.Interceptor = interceptor;

            return result;
        }

        /// <summary>
        /// The create proxy type.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The array of base interfaces.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        internal Type CreateProxyType(Type baseType, params Type[] baseInterfaces)
        {
            try
            {
                TypeMapLock.EnterWriteLock();

                // Reuse the previous results, if possible
                if (this.proxyCache != null && this.proxyCache.Contains(baseType, baseInterfaces))
                {
                    return this.proxyCache.GetProxyType(baseType, baseInterfaces);
                }

                Type result = this.CreateUncachedProxyType(baseInterfaces, baseType);

                // Cache the proxy type
                if (result != null && this.proxyCache != null)
                {
                    this.proxyCache.StoreProxyType(result, baseType, baseInterfaces);
                }

                return result;
            }
            finally
            {
                TypeMapLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Builds a list of interface types that the type implements.
        /// </summary>
        /// <param name="currentType">
        /// The current type.
        /// </param>
        /// <param name="interfaceList">
        /// The interface list.
        /// </param>
        private static void BuildInterfaceList(Type currentType, List<Type> interfaceList)
        {
            Type[] interfaces = currentType.GetInterfaces();
            if (interfaces.Length == 0)
            {
                return;
            }

            foreach (Type current in interfaces)
            {
                if (interfaceList.Contains(current))
                {
                    continue;
                }

                interfaceList.Add(current);
                BuildInterfaceList(current, interfaceList);
            }
        }

        /// <summary>
        /// Defines the default constructor so that types without one can be created.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        /// <returns>
        /// The <see cref="ConstructorBuilder"/>.
        /// </returns>
        private static ConstructorBuilder DefineConstructor(TypeBuilder typeBuilder)
        {
            const MethodAttributes ConstructorAttributes = MethodAttributes.Public |
                                                           MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                                           MethodAttributes.RTSpecialName;

            ConstructorBuilder constructor =
                typeBuilder.DefineConstructor(ConstructorAttributes, CallingConventions.Standard, new Type[] { });

            ILGenerator il = constructor.GetILGenerator();

            constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, BaseConstructor);
            il.Emit(OpCodes.Ret);

            return constructor;
        }

        /// <summary>
        /// Builds a list of methods and properties within the type that can be intercepted.
        /// </summary>
        /// <param name="interfaceList">
        /// The interface list.
        /// </param>
        /// <param name="methods">
        /// The methods.
        /// </param>
        /// <param name="proxyList">
        /// The proxy list.
        /// </param>
        private static void BuildMethodList(IEnumerable<Type> interfaceList, IEnumerable<MethodInfo> methods, List<MethodInfo> proxyList)
        {
            foreach (MethodInfo method in methods)
            {
                // Only non-private methods will be proxied
                if (method.IsPrivate)
                {
                    continue;
                }

                // Final methods cannot be overridden
                if (method.IsFinal)
                {
                    continue;
                }

                // Only virtual methods can be intercepted
                if (!method.IsVirtual && !method.IsAbstract)
                {
                    continue;
                }

                proxyList.Add(method);
            }

            foreach (Type interfaceType in interfaceList)
            {
                MethodInfo[] interfaceMethods = interfaceType.GetMethods();
                foreach (MethodInfo interfaceMethod in interfaceMethods)
                {
                    if (proxyList.Contains(interfaceMethod))
                    {
                        continue;
                    }

                    proxyList.Add(interfaceMethod);
                }
            }
        }

        /// <summary>
        /// Defines the default serialization constructor so that types without one can be created.
        /// </summary>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        /// <param name="interceptorField">
        /// The interceptor field.
        /// </param>
        /// <param name="defaultConstructor">
        /// The default constructor.
        /// </param>
        private static void DefineSerializationConstructor(Type[] baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
        {
            const MethodAttributes ConstructorAttributes = MethodAttributes.Public |
                                                           MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                                           MethodAttributes.RTSpecialName;

            Type[] parameterTypes = { typeof(SerializationInfo), typeof(StreamingContext) };
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                ConstructorAttributes,
                CallingConventions.Standard,
                parameterTypes);

            ILGenerator il = constructor.GetILGenerator();

            LocalBuilder interceptorType = il.DeclareLocal(typeof(Type));

            constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

            il.Emit(OpCodes.Ldtoken, typeof(IInterceptor));
            il.Emit(OpCodes.Call, GetTypeFromHandle);
            il.Emit(OpCodes.Stloc, interceptorType);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, defaultConstructor);

            // __interceptor = (IInterceptor)info.GetValue("__interceptor", typeof(IInterceptor));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, "__interceptor");
            il.Emit(OpCodes.Ldloc, interceptorType);
            il.Emit(OpCodes.Callvirt, GetValue);
            il.Emit(OpCodes.Castclass, typeof(IInterceptor));
            il.Emit(OpCodes.Stfld, interceptorField);

            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Adds serialization support to the current proxy.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        /// <param name="interceptorField">
        /// The interceptor field.
        /// </param>
        /// <param name="defaultConstructor">
        /// The default constructor.
        /// </param>
        private static void AddSerializationSupport(Type baseType, Type[] baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
        {
            ConstructorInfo serializableConstructor = typeof(SerializableAttribute).GetConstructor(new Type[0]);
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, new object[0]);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);

            DefineSerializationConstructor(baseInterfaces, typeBuilder, interceptorField, defaultConstructor);
            ImplementGetObjectData(baseType, baseInterfaces, typeBuilder, interceptorField);

        }

        /// <summary>
        /// Implements the method to get the current objects data.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        /// <param name="interceptorField">
        /// The interceptor field.
        /// </param>
        private static void ImplementGetObjectData(Type baseType, Type[] baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField)
        {
            const MethodAttributes Attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                MethodAttributes.Virtual;
            Type[] parameterTypes = { typeof(SerializationInfo), typeof(StreamingContext) };

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("GetObjectData", Attributes, typeof(void), parameterTypes);

            ILGenerator il = methodBuilder.GetILGenerator();

            // info.SetType(typeof(ProxyObjectReference));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldtoken, typeof(ProxyObjectReference));
            il.Emit(OpCodes.Call, GetTypeFromHandle);
            il.Emit(OpCodes.Callvirt, SetType);

            // info.AddValue("__interceptor", __interceptor);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, "__interceptor");
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, interceptorField);
            il.Emit(OpCodes.Callvirt, AddValue);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, "__baseType");
            il.Emit(OpCodes.Ldstr, baseType.AssemblyQualifiedName);
            il.Emit(OpCodes.Callvirt, AddValue);

            var interfaces = baseInterfaces ?? new Type[0];
            int baseInterfaceCount = interfaces.Length;

            // Save the number of base interfaces
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, "__baseInterfaceCount");
            il.Emit(OpCodes.Ldc_I4, baseInterfaceCount);
            il.Emit(OpCodes.Box, typeof(int));
            il.Emit(OpCodes.Callvirt, AddValue);

            int index = 0;
            foreach (Type baseInterface in interfaces)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, string.Format("__baseInterface{0}", index++));
                il.Emit(OpCodes.Ldstr, baseInterface.AssemblyQualifiedName);
                il.Emit(OpCodes.Callvirt, AddValue);
            }

            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Creates an un-cached instance of the proxy type.
        /// </summary>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        private Type CreateUncachedProxyType(Type[] baseInterfaces, Type baseType)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string typeName = string.Format("{0}Proxy", baseType.Name);
            string assemblyName = string.Format("{0}Assembly", typeName);
            string moduleName = string.Format("{0}Module", typeName);

            AssemblyName name = new AssemblyName(assemblyName);
#if DEBUG
            AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif
            AssemblyBuilder assemblyBuilder = currentDomain.DefineDynamicAssembly(name, access);

#if DEBUG
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);
#else
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif

            const TypeAttributes TypeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class |
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

            List<Type> interfaceList = new List<Type>();
            if (baseInterfaces != null && baseInterfaces.Length > 0)
            {
                interfaceList.AddRange(baseInterfaces);
            }

            // Use the proxy dummy as the base type 
            // since we're not inheriting from any class type
            Type parentType = baseType;
            if (baseType.IsInterface)
            {
                parentType = typeof(ProxyDummyBase);
                interfaceList.Add(baseType);
            }

            // Add any inherited interfaces
            Type[] interfaces = interfaceList.ToArray();
            foreach (Type interfaceType in interfaces)
            {
                BuildInterfaceList(interfaceType, interfaceList);
            }

            // Add the ISerializable interface so that it can be implemented
            if (!interfaceList.Contains(typeof(ISerializable)))
            {
                interfaceList.Add(typeof(ISerializable));
            }

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes, parentType, interfaceList.ToArray());

            ConstructorBuilder defaultConstructor = DefineConstructor(typeBuilder);

            // Implement IProxy
            ProxyImplementer implementor = new ProxyImplementer();
            implementor.ImplementProxy(typeBuilder);

            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<MethodInfo> proxyList = new List<MethodInfo>();
            BuildMethodList(interfaceList, methods, proxyList);

            FieldInfo interceptorField = implementor.InterceptorField;
            foreach (MethodInfo method in proxyList)
            {
                // Provide a custom implementation of ISerializable
                // instead of redirecting it back to the interceptor
                if (method.DeclaringType == typeof(ISerializable))
                {
                    continue;
                }

                this.proxyMethodBuilder.CreateProxiedMethod(interceptorField, method, typeBuilder);
            }

            // Make the proxy serializable
            AddSerializationSupport(baseType, baseInterfaces, typeBuilder, interceptorField, defaultConstructor);

            Type proxyType = typeBuilder.CreateType();

#if DEBUG
            assemblyBuilder.Save("LazyProxyModule.dll");
#endif
            return proxyType;
        }
    }
}
