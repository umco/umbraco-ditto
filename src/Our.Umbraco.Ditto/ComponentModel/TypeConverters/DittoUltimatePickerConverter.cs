namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides a unified way of converting ultimate picker properties to strong typed collections.
    /// </summary>
    public class DittoUltimatePickerConverter : DittoUmbracoBaseConverter
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
                || sourceType == typeof(int))
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
                // There's no way to determine the type here.
                return null;
            }

            var propertyType = context.PropertyDescriptor.PropertyType;
            var isGenericType = propertyType.IsGenericType;
            var targetType = isGenericType
                                ? propertyType.GenericTypeArguments.First()
                                : propertyType;

            if (IsNullOrEmptyString(value))
            {
                if (isGenericType)
                {
                    return EnumerableInvocations.Empty(targetType);
                }

                return null;
            }

            // If a single item is selected, this comes back as an int, not a string.
            if (value is int)
            {
                var id = (int)value;
                var umbracoHelper = UmbracoHelper;

                // CheckBoxList, ListBox
                if (targetType != null)
                {
                    return umbracoHelper.TypedContent(id)
                                        .As(targetType, null, null, culture).YieldSingleItem();
                }

                // AutoComplete, DropDownList, RadioButton
                return umbracoHelper.TypedContent(id).As(propertyType, null, null, culture);
            }

            string s = value as string ?? value.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                int n;
                var nodeIds = s
                    .ToDelimitedList()
                    .Select(x => int.TryParse(x, NumberStyles.Any, culture, out n) ? n : -1)
                    .Where(x => x > 0)
                    .ToArray();

                if (nodeIds.Any())
                {
                    var umbracoHelper = UmbracoHelper;
                    var ultimatePicker = new List<IPublishedContent>();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var nodeId in nodeIds)
                    {
                        var item = umbracoHelper.TypedContent(nodeId);

                        if (item != null)
                        {
                            ultimatePicker.Add(item);
                        }
                    }

                    // CheckBoxList, ListBox
                    if (isGenericType)
                    {
                        return ultimatePicker.As(targetType, null, null, null, culture);
                    }

                    // AutoComplete, DropDownList, RadioButton
                    return ultimatePicker.As(targetType, null, null, null, culture).FirstOrDefault();
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
