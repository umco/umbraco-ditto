namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The proxy object reference.
    /// </summary>
    [Serializable]
    public class ProxyObjectReference : IObjectReference, ISerializable
    {
        /// <summary>
        /// The proxy.
        /// </summary>
        private readonly IProxy proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyObjectReference"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> storing all the data needed to serialize the object.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/>.
        /// </param>
        protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
        {
            // Deserialize the base type using its assembly qualified name
            string qualifiedName = info.GetString("__baseType");
            Type baseType = Type.GetType(qualifiedName, true, false);

            // Rebuild the list of interfaces
            List<Type> interfaceList = new List<Type>();
            int interfaceCount = info.GetInt32("__baseInterfaceCount");
            for (int i = 0; i < interfaceCount; i++)
            {
                string keyName = string.Format("__baseInterface{0}", i);
                string currentQualifiedName = info.GetString(keyName);
                Type interfaceType = Type.GetType(currentQualifiedName, true, false);

                interfaceList.Add(interfaceType);
            }

            // Reconstruct the proxy
            LazyProxyFactory factory = new LazyProxyFactory();
            Type proxyType = factory.CreateProxyType(baseType, interfaceList.ToArray());

            // Initialize the proxy with the deserialized data
            object[] args = new object[] { info, context };
            this.proxy = (IProxy)Activator.CreateInstance(proxyType, args);
        }

        /// <summary>
        /// Returns the real object that should be deserialized, rather than the object 
        /// that the serialized stream specifies.
        /// </summary>
        /// <returns>
        /// Returns the actual object that is put into the graph.
        /// </returns>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> from 
        /// which the current object is deserialized. </param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission. 
        /// The call will not work on a medium trusted server.
        /// </exception>
        public object GetRealObject(StreamingContext context)
        {
            return this.proxy;
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data 
        /// needed to serialize the target object.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">
        /// The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
