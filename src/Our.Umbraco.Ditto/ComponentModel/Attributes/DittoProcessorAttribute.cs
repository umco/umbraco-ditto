using System;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The abstract base class for all Ditto processor attributes
    /// </summary>
    [AttributeUsage(Ditto.ProcessorAttributeTargets, AllowMultiple = true)]
    [DittoProcessorMetaData(ValueType = typeof(object), ContextType = typeof(DittoProcessorContext))]
    public abstract class DittoProcessorAttribute : DittoCacheableAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoProcessorAttribute"/> class.
        /// </summary>
        protected DittoProcessorAttribute()
        {
            var metaData = this.GetType().GetCustomAttribute<DittoProcessorMetaDataAttribute>(true);
            if (metaData == null)
            {
                throw new ApplicationException("Ditto processor attributes require a DittoProcessorMetaData attribute to be applied to the class but none was found");
            }

            this.ValueType = metaData.ValueType;
            this.ContextType = metaData.ContextType;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Value { get; protected set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DittoProcessorContext Context { get; protected set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        internal Type ValueType { get; set; }

        /// <summary>
        /// Gets or sets the type of the context.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        internal Type ContextType { get; set; }

        /// <summary>
        /// Returns the MembershipHelper instance
        /// </summary>
        protected MembershipHelper Members
        {
            get { return new MembershipHelper(UmbracoContext); }
        }

        /// <summary>
        /// Returns the UmbracoHelper instance
        /// </summary>
        protected UmbracoHelper Umbraco
        {
            get { return new UmbracoHelper(UmbracoContext); }
        }

        /// <summary>
        /// Returns an ILogger
        /// </summary>
        protected ILogger Logger
        {
            get { return ProfilingLogger.Logger; }
        }

        /// <summary>
        /// Returns a ProfilingLogger
        /// </summary>
        protected virtual ProfilingLogger ProfilingLogger
        {
            get { return ApplicationContext.ProfilingLogger; }
        }

        /// <summary>
        /// Returns the current UmbracoContext
        /// </summary>
        public virtual UmbracoContext UmbracoContext { get; internal set; }

        /// <summary>
        /// Returns the current ApplicationContext
        /// </summary>
        public virtual ApplicationContext ApplicationContext { get; internal set; }

        /// <summary>
        /// Returns a ServiceContext
        /// </summary>
        protected ServiceContext Services
        {
            get { return ApplicationContext.Services; }
        }

        /// <summary>
        /// Returns a DatabaseContext
        /// </summary>
        protected DatabaseContext DatabaseContext
        {
            get { return ApplicationContext.DatabaseContext; }
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public abstract object ProcessValue();

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        internal virtual object ProcessValue(
            object value,
            DittoProcessorContext context)
        {
            if (value != null && !this.ValueType.IsInstanceOfType(value))
            {
                throw new ArgumentException("Expected a value argument of type " + this.ValueType + " but got " + value.GetType(), "value");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!this.ContextType.IsInstanceOfType(context))
            {
                throw new ArgumentException("Expected a context argument of type " + this.ContextType + " but got " + context.GetType(), "context");
            }

            this.Value = value;
            this.Context = context;

            var ctx = new DittoCacheContext(this, context.Content, context.TargetType, context.PropertyDescriptor, context.Culture);
            return this.GetCacheItem(ctx, this.ProcessValue);
        }
    }
}