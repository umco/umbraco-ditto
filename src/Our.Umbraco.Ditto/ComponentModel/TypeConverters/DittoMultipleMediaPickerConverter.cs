namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides a unified way of converting multi media picker properties to strong typed collections.
    /// </summary>
    public class DittoMultipleMediaPickerConverter : DittoConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // We can pass null here.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (sourceType == null
                || sourceType == typeof(string)
                || sourceType == typeof(int)
                || typeof(IPublishedContent).IsAssignableFrom(sourceType))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (context == null || context.PropertyDescriptor == null)
            {
                return Enumerable.Empty<object>();
            }

            var propertyType = context.PropertyDescriptor.PropertyType;
            var isGenericType = propertyType.IsGenericType;
            var targetType = isGenericType
                                ? propertyType.GenericTypeArguments.First()
                                : propertyType;

            if (value.IsNullOrEmptyString())
            {
                return EnumerableInvocations.Empty(targetType);
            }

            // DictionaryPublishedContent 
            IPublishedContent content = value as IPublishedContent;
            if (content != null)
            {
                // Use the id so we get folder sanitation.
                return this.ConvertMediaFromInt(content.Id, targetType, culture);
            }

            // If a single item is selected, this is passed as an int, not a string.
            if (value is int)
            {
                var id = (int)value;
                return ConvertMediaFromInt(id, targetType, culture).YieldSingleItem();
            }

            var s = value as string;
            if (!string.IsNullOrWhiteSpace(s))
            {
                var multiMediaPicker = EnumerableInvocations.Empty(targetType);

                int n;
                var nodeIds =
                    XmlHelper.CouldItBeXml(s)
                    ? s.GetXmlIds()
                    : s
                        .ToDelimitedList()
                        .Select(x => int.TryParse(x, NumberStyles.Any, culture, out n) ? n : -1)
                        .Where(x => x > 0)
                        .ToArray();

                if (nodeIds.Any())
                {
                    multiMediaPicker = nodeIds.ForEach(i => this.ConvertMediaFromInt(i, targetType, culture));
                }

                return multiMediaPicker;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}