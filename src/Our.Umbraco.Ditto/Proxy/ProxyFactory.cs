using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The proxy factory for creating instances of proxy classes.
    /// </summary>
    public class ProxyFactory
    {
        /// <summary>
        /// Ensures that proxy creation is atomic.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        /// <summary>
        /// The proxy cache for storing proxy types.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> ProxyCache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Creates an instance of the proxy class for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="baseType">The base <see cref="Type"/> to proxy.</param>
        /// <param name="properties">The collection of property names to map.</param>
        /// <param name="content">The <see cref="IPublishedContent"/> to pass as a parameter.</param>
        /// <returns>
        /// The proxy <see cref="Type"/> instance.
        /// </returns>
        public IProxy CreateProxy(Type baseType, IEnumerable<string> properties, IPublishedContent content = null)
        {
            Type proxyType = this.CreateProxyType(baseType, properties);

            object result = content == null ? proxyType.GetInstance() : proxyType.GetInstance(content);

            return (IProxy)result;
        }

        /// <summary>
        /// Creates the proxy class or returns already created class from the cache.
        /// </summary>
        /// <param name="baseType">The base <see cref="Type"/> to proxy.</param>
        /// <param name="properties">The collection of property names to map.</param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        private Type CreateProxyType(Type baseType, IEnumerable<string> properties)
        {
            try
            {
                // ConcurrentDictionary.GetOrAdd() is not atomic so we'll be doubly sure.
                Locker.EnterWriteLock();

                return ProxyCache.GetOrAdd(baseType, c => this.CreateUncachedProxyType(baseType, properties));
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Creates an un-cached proxy class.
        /// </summary>
        /// <param name="baseType">The base <see cref="Type"/> to proxy.</param>
        /// <param name="properties">The collection of property names to map.</param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        private Type CreateUncachedProxyType(Type baseType, IEnumerable<string> properties)
        {
            // Create a dynamic assembly and module to store the proxy.
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string typeName = $"{baseType.Name}Proxy";
            string assemblyName = $"{typeName}Assembly";
            string moduleName = $"{typeName}Module";

            // Define different behaviors for debug and release so that we can make debugging easier.
            var name = new AssemblyName(assemblyName);
            AssemblyBuilder assemblyBuilder;
#if DEBUG
            assemblyBuilder = currentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, $"{moduleName}.mod", true);

            // Add a debuggable attribute to the assembly saying to disable optimizations
            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new[] { typeof(DebuggableAttribute.DebuggingModes) });
            var daBuilder = new CustomAttributeBuilder(
                daCtor,
                new object[] { DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.Default });
            assemblyBuilder.SetCustomAttribute(daBuilder);

#else
            assemblyBuilder = currentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif

            // Define type attributes
            const TypeAttributes typeAttributes = TypeAttributes.AutoClass |
                                                  TypeAttributes.Class |
                                                  TypeAttributes.Public |
                                                  TypeAttributes.BeforeFieldInit;

            // Define the type.
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, baseType);

            // Emit the default constructors for this proxy so that classes without parameterless constructors
            // can be proxied.
            ConstructorInfo[] constructors = baseType.GetConstructors();
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                ConstructorEmitter.Emit(typeBuilder, constructorInfo);
            }

            // Emit the IProxy IInterceptor property.
            FieldInfo interceptorField = InterceptorEmitter.Emit(typeBuilder);

            // Collect and filter our list of properties to intercept.
            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            IEnumerable<MethodInfo> proxyList = this.BuildPropertyList(methods)
                .Where(m => properties.Contains(m.Name.Substring(4), StringComparer.OrdinalIgnoreCase));

            // Emit each property that is to be intercepted.
            foreach (MethodInfo methodInfo in proxyList)
            {
                PropertyEmitter.Emit(typeBuilder, methodInfo, interceptorField);
            }

            // Create and return.
            Type result = typeBuilder.CreateType();

#if DEBUG
            assemblyBuilder.Save(typeName + ".dll");
#endif
            return result;
        }

        /// <summary>
        /// Returns the <see cref="IEnumerable{MethodInfo}"/> representing the properties to exclude.
        /// </summary>
        /// <param name="methodInfos">
        /// The <see cref="IEnumerable{MethodInfo}"/> representing all methods and properties on the base class to proxy.
        /// </param>
        /// <returns>
        /// The filtered <see cref="IEnumerable{MethodInfo}"/>.
        /// </returns>
        private IEnumerable<MethodInfo> BuildPropertyList(MethodInfo[] methodInfos)
        {
            var proxyList = new List<MethodInfo>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (MethodInfo method in methodInfos)
            {
                // Only non-private methods will be intercepted.
                if (method.IsPrivate)
                {
                    continue;
                }

                // Final methods cannot be overridden.
                if (method.IsFinal)
                {
                    continue;
                }

                // Only virtual methods can be intercepted.
                if (method.IsVirtual == false && method.IsAbstract == false)
                {
                    continue;
                }

                // We only want properties not methods that are not part of the excluded list.
                PropertyInfo property = this.GetParentProperty(method);
                if (property != null)
                {
                    proxyList.Add(method);
                }
            }

            return proxyList;
        }

        /// <summary>
        /// Returns the parent <see cref="PropertyInfo"/> if any, for the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/>.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/>, or <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <see cref="MethodInfo"/> is <c>null</c>.
        /// </exception>
        private PropertyInfo GetParentProperty(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance;

            bool takesArg = method.GetParameters().Length == 1;
            bool hasReturn = method.ReturnType != typeof(void);

            if ((takesArg || hasReturn) == false)
            {
                return null;
            }

            if (takesArg && hasReturn == false)
            {
                if (method.DeclaringType != null)
                {
                    return method.DeclaringType.GetProperties(propertyFlags)
                        .FirstOrDefault(p => this.AreMethodsEqualForDeclaringType(p.GetSetMethod(), method));
                }
            }

            if (method.DeclaringType != null)
            {
                return method.DeclaringType.GetProperties(propertyFlags)
                             .FirstOrDefault(p => this.AreMethodsEqualForDeclaringType(p.GetGetMethod(), method));
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether two instances of <see cref="MethodInfo"/> are equal
        /// for a declaring type.
        /// </summary>
        /// <param name="first">The first <see cref="MethodInfo"/>.</param>
        /// <param name="second">The second <see cref="MethodInfo"/>.</param>
        /// <returns>
        /// True if the two instances of <see cref="MethodInfo"/> are equal; otherwise, false.
        /// </returns>
        private bool AreMethodsEqualForDeclaringType(MethodInfo first, MethodInfo second)
        {
            byte[] firstBytes = { };
            byte[] secondBytes = { };

            if (first != null && first.ReflectedType != null && first.DeclaringType != null)
            {
                first = first.ReflectedType == first.DeclaringType ? first
                            : first.DeclaringType.GetMethod(
                                first.Name,
                                first.GetParameters().Select(p => p.ParameterType).ToArray());

                MethodBody body = first.GetMethodBody();
                firstBytes = body.GetILAsByteArray();
            }

            if (second != null && second.ReflectedType != null && second.DeclaringType != null)
            {
                second = second.ReflectedType == second.DeclaringType
                             ? second
                             : second.DeclaringType.GetMethod(
                                 second.Name,
                                 second.GetParameters().Select(p => p.ParameterType).ToArray());

                MethodBody body = second.GetMethodBody();
                secondBytes = body.GetILAsByteArray();
            }

            return firstBytes.SequenceEqual(secondBytes);
        }
    }
}