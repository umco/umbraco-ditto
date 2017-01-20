using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Provides a unified way of converting ultimate picker properties to strong typed collections.
    /// TODO: [JMJS] Do we need this anymore since we are not supporting Umbraco 6?
    /// </summary>
    public class UltimatePickerAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ProcessValue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this.Context == null || this.Context.PropertyDescriptor == null)
            {
                // There's no way to determine the type here.
                return null;
            }

            var propertyType = this.Context.PropertyDescriptor.PropertyType;
            var isGenericType = propertyType.IsGenericType;
            var targetType = isGenericType
                                ? propertyType.GenericTypeArguments.First()
                                : propertyType;

            if (this.Value.IsNullOrEmptyString())
            {
                if (isGenericType)
                {
                    return EnumerableInvocations.Empty(targetType);
                }

                return null;
            }

            // If a single item is selected, this comes back as an int, not a string.
            if (this.Value is int)
            {
                var id = (int)this.Value;

                // CheckBoxList, ListBox
                if (targetType != null)
                {
                    return this.ConvertContentFromInt(id, targetType, this.Context.Culture).YieldSingleItem();
                }

                // AutoComplete, DropDownList, RadioButton
                return this.ConvertContentFromInt(id, propertyType, this.Context.Culture);
            }

            if (this.Value != null)
            {
                string s = this.Value as string ?? this.Value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    int n;
                    var nodeIds = s.ToDelimitedList()
                            .Select(x => int.TryParse(x, NumberStyles.Any, this.Context.Culture, out n) ? n : -1)
                            .Where(x => x > 0)
                            .ToArray();

                    if (nodeIds.Any())
                    {
                        var ultimatePicker = new List<IPublishedContent>();

                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (var nodeId in nodeIds)
                        {
                            var item = Umbraco.ContentCache.GetById(nodeId);

                            if (item != null)
                            {
                                ultimatePicker.Add(item);
                            }
                        }

                        // CheckBoxList, ListBox
                        if (isGenericType)
                        {
                            return ultimatePicker.As(targetType, this.Context.Culture);
                        }

                        // AutoComplete, DropDownList, RadioButton
                        return ultimatePicker.As(targetType, this.Context.Culture).FirstOrDefault();
                    }
                }
            }

            return null;
        }
    }
}