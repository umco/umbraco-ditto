using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Our.Umbraco.Ditto
{
    public abstract class DittoProcessor
    {
        public object Value { get; protected set; }

        public DittoProcessorContext Context { get; protected set; }

        public DittoProcessorAttribute Attribute { get; protected set; }

        public CultureInfo Culture { get; protected set; }

        public abstract object ProcessValue();

        internal virtual object ProcessValue(
            DittoProcessorContext context,
            DittoProcessorAttribute attribute,
            CultureInfo culture)
        {
            Value = context.Value;
            Context = context;
            Attribute = attribute;
            Culture = culture;

            return this.ProcessValue();
        }
    }

    public abstract class DittoProcessor<TValueType> : DittoProcessor
    {
        public new TValueType Value 
        {
            get
            {
                return (TValueType)base.Value;
            }
            protected set
            {
                base.Value = value;
            }
        }
    }

    public abstract class DittoProcessor<TValueType, TContextType> :
        DittoProcessor<TValueType, TContextType, DittoProcessorAttribute>
        where TContextType : DittoProcessorContext
    { }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class DittoProcessor<TValueType, TContextType, TAttributeType> : DittoProcessor<TValueType>
        where TContextType : DittoProcessorContext
        where TAttributeType : DittoProcessorAttribute
    {
        public new TContextType Context
        {
            get
            {
                return (TContextType)base.Context;
            }
            protected set
            {
                base.Context = value;
            }
        }

        public new TAttributeType Attribute
        {
            get
            {
                return (TAttributeType)base.Attribute;
            }
            protected set
            {
                base.Attribute = value;
            }
        }
    }
}