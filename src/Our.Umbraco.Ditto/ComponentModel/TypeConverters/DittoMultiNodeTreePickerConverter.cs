namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Security;

    /// <summary>
    /// Provides a unified way of converting multi node tree picker properties to strong typed collections.
    /// Adapted from <see href="https://github.com/Jeavon/Umbraco-Core-Property-Value-Converters/blob/v2/Our.Umbraco.PropertyConverters/MultiNodeTreePickerPropertyConverter.cs"/>
    /// </summary>
    public class DittoMultiNodeTreePickerConverter : DittoConverter
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
                || typeof(IPublishedContent).IsAssignableFrom(sourceType)
                || sourceType.IsEnumerableOfType(typeof(IPublishedContent))
                || sourceType.IsEnumerableOfType(typeof(string))
                || sourceType.IsEnumerableOfType(typeof(int)))
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

            // Single IPublishedContent 
            IPublishedContent content = value as IPublishedContent;
            if (content != null)
            {
                return content.As(targetType, null, null, culture);
            }

            // ReSharper disable once PossibleNullReferenceException
            var type = value.GetType();

            // Multiple IPublishedContent 
            if (type.IsEnumerableOfType(typeof(IPublishedContent)))
            {
                return ((IEnumerable<IPublishedContent>)value).As(targetType, null, null, null, culture);
            }

            int[] nodeIds = { };

            // First try enumerable strings, ints.
            if (type.IsGenericType || type.IsArray)
            {
                if (type.IsEnumerableOfType(typeof(string)))
                {
                    int n;
                    nodeIds = ((IEnumerable<string>)value)
                                  .Select(x => int.TryParse(x, NumberStyles.Any, culture, out n) ? n : -1)
                                  .ToArray();
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = ((IEnumerable<int>)value).ToArray();
                }
            }

            // Now csv strings.
            if (!nodeIds.Any())
            {
                var s = value as string ?? value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    int n;
                    nodeIds = XmlHelper.CouldItBeXml(s)
                    ? s.GetXmlIds()
                    : s
                    .ToDelimitedList()
                    .Select(x => int.TryParse(x, NumberStyles.Any, culture, out n) ? n : -1)
                    .Where(x => x > 0)
                    .ToArray();
                }
            }

            if (nodeIds.Any())
            {
                var umbracoContext = UmbracoContext.Current;
                var membershipHelper = new MembershipHelper(umbracoContext);
                var objectType = UmbracoObjectTypes.Unknown;
                var multiPicker = new List<IPublishedContent>();

                // Oh so ugly if you let Resharper do this.
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var nodeId in nodeIds)
                {
                    var item = this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Document, umbracoContext.ContentCache.GetById)
                         ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Media, umbracoContext.MediaCache.GetById)
                         ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Member, membershipHelper.GetById);

                    if (item != null)
                    {
                        multiPicker.Add(item);
                    }
                }

                return multiPicker.As(targetType, null, null, null, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempt to get an <see cref="IPublishedContent"/> instance based on id and object type.
        /// </summary>
        /// <param name="nodeId">The content node ID</param>
        /// <param name="actual">The type of content being requested</param>
        /// <param name="expected">The type of content expected/supported by <paramref name="typedMethod"/></param>
        /// <param name="typedMethod">A function to fetch content of type <paramref name="expected"/></param>
        /// <returns>
        /// The requested content, or null if either it does not exist or <paramref name="actual"/> does not 
        /// match <paramref name="expected"/>
        /// </returns>
        private IPublishedContent GetPublishedContent(int nodeId, ref UmbracoObjectTypes actual, UmbracoObjectTypes expected, Func<int, IPublishedContent> typedMethod)
        {
            // Is the given type supported by the typed method.
            if (actual != UmbracoObjectTypes.Unknown && actual != expected)
            {
                return null;
            }

            // Attempt to get the content
            var content = typedMethod(nodeId);
            if (content != null)
            {
                // If we find the content, assign the expected type to the actual type so we don't have to 
                // keep looking for other types of content.
                actual = expected;
            }

            return content;
        }
    }
}