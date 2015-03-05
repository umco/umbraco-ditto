namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The published content context for supplying contextual information about the 
    /// <see cref="IPublishedContent"/> that is undergoing conversion.
    /// </summary>
    internal class PublishedContentContext : ITypeDescriptorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedContentContext"/> class.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="IPublishedContent"/> that is being converted from.
        /// </param>
        /// <param name="descriptor">
        /// The <see cref="PropertyDescriptor"/> for the property on the target type that contains
        /// the currently converting property.
        /// </param>
        public PublishedContentContext(object instance, PropertyDescriptor descriptor)
        {
            this.Instance = instance;

            // This is the property descriptor for the property on the POCO that is 
            // getting converted.
            this.PropertyDescriptor = descriptor;
        }

        /// <summary>
        /// Gets the object that is connected with this type descriptor request.
        /// </summary>
        /// <returns>
        /// The object that invokes the method on the <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no object responsible for the call.
        /// </returns>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.ComponentModel.PropertyDescriptor"/> that is associated with the given context item.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptor"/> that describes the given context item; otherwise, null if there is no <see cref="T:System.ComponentModel.PropertyDescriptor"/> responsible for the call.
        /// </returns>
        public PropertyDescriptor PropertyDescriptor { get; private set; }

        /// <summary>
        /// Gets the container representing this <see cref="T:System.ComponentModel.TypeDescriptor"/> request.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.IContainer"/> with the set of objects for this <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no container or if the <see cref="T:System.ComponentModel.TypeDescriptor"/> does not use outside objects.
        /// </returns>
        public IContainer Container
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanged"/> event.
        /// </summary>
        public void OnComponentChanged()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanging"/> event.
        /// </summary>
        /// <returns>
        /// true if this object can be changed; otherwise, false.
        /// </returns>
        public bool OnComponentChanging()
        {
            return true;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
