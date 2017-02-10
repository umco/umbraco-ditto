using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Umbraco.Core.Models;

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
        /// <param name="interceptor">The <see cref="IInterceptor"/> to intercept properties with.</param>
        /// <param name="content">The <see cref="IPublishedContent"/> to pass as a parameter.</param>
        /// <returns>
        /// The proxy <see cref="Type"/> instance.
        /// </returns>
        public object CreateProxy(Type baseType, IInterceptor interceptor, IPublishedContent content = null)
        {
            Type proxyType = this.CreateProxyType(baseType);

            object result = content == null ? proxyType.GetInstance() : proxyType.GetInstance(content);

            IProxy proxy = (IProxy)result;
            proxy.Interceptor = interceptor;

            return result;
        }

        /// <summary>
        /// Creates the proxy class or returns already created class from the cache.
        /// </summary>
        /// <param name="baseType">The base <see cref="Type"/> to proxy.</param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        private Type CreateProxyType(Type baseType)
        {
            try
            {
                // ConcurrentDictionary.GetOrAdd() is not atomic so we'll be doubly sure.
                Locker.EnterWriteLock();

                return ProxyCache.GetOrAdd(baseType, c => this.CreateUncachedProxyType(baseType));
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Creates an un-cached proxy class.
        /// </summary>
        /// <param name="baseType">
        /// The base <see cref="Type"/> to proxy.
        /// </param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        private Type CreateUncachedProxyType(Type baseType)
        {
            // Create a dynamic assembly and module to store the proxy.
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string typeName = string.Format("{0}Proxy", baseType.Name);
            string assemblyName = string.Format("{0}Assembly", typeName);
            string moduleName = string.Format("{0}Module", typeName);

            // Define different behaviors for debug and release so that we can make debugging easier.
            AssemblyName name = new AssemblyName(assemblyName);
#if DEBUG
            AssemblyBuilder assemblyBuilder = currentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);

#else
            AssemblyBuilder assemblyBuilder = currentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
            // Define type attributes
            const TypeAttributes TypeAttributes = TypeAttributes.AutoClass |
                                                  TypeAttributes.Class |
                                                  TypeAttributes.Public |
                                                  TypeAttributes.BeforeFieldInit;

            // Define the type.
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes, baseType);

            // Emit the default constructors for this proxy so that classes without parameterless constructors
            // can be proxied.
            ConstructorInfo[] constructors = baseType.GetConstructors();
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                ConstructorEmitter.Emit(typeBuilder, constructorInfo);
            }

            // Emit the IProxy IInterceptor property.
            FieldInfo interceptorField = InterceptorEmitter.Emit(typeBuilder);

            // Emit each property that is to be intercepted.
            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<MethodInfo> proxyList = this.BuildPropertyList(methods);

            foreach (MethodInfo methodInfo in proxyList)
            {
                PropertyEmitter.Emit(typeBuilder, methodInfo, interceptorField);
            }

            // Create and return.
            return typeBuilder.CreateType();
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
            List<MethodInfo> proxyList = new List<MethodInfo>();

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
                if (!method.IsVirtual && !method.IsAbstract)
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
        /// <param name="method">
        /// The <see cref="MethodInfo"/>.
        /// </param>
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
                throw new ArgumentNullException("method");
            }

            const BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public;

            bool takesArg = method.GetParameters().Length == 1;
            bool hasReturn = method.ReturnType != typeof(void);

            if (!(takesArg || hasReturn))
            {
                return null;
            }

            if (takesArg && !hasReturn)
            {
                if (method.DeclaringType != null)
                {
                    return method.DeclaringType.GetProperties(PropertyFlags)
                                 .FirstOrDefault(p => this.AreMethodsEqualForDeclaringType(p.GetSetMethod(), method));
                }
            }

            if (method.DeclaringType != null)
            {
                return method.DeclaringType.GetProperties(PropertyFlags)
                             .FirstOrDefault(p => this.AreMethodsEqualForDeclaringType(p.GetGetMethod(), method));
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether two instances of <see cref="MethodInfo"/> are equal 
        /// for a declaring type.
        /// </summary>
        /// <param name="first">
        /// The first <see cref="MethodInfo"/>.
        /// </param>
        /// <param name="second">
        /// The second <see cref="MethodInfo"/>.
        /// </param>
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
                if (body != null)
                {
                    firstBytes = body.GetILAsByteArray();
                }
            }

            if (second != null && second.ReflectedType != null && second.DeclaringType != null)
            {
                second = second.ReflectedType == second.DeclaringType
                             ? second
                             : second.DeclaringType.GetMethod(
                                 second.Name,
                                 second.GetParameters().Select(p => p.ParameterType).ToArray());

                MethodBody body = second.GetMethodBody();
                if (body != null)
                {
                    secondBytes = body.GetILAsByteArray();
                }
            }

            return firstBytes.SequenceEqual(secondBytes);
        }
    }
}