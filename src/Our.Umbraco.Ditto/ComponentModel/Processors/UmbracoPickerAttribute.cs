using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A picker processor for handling the various types of umbaco pickers
    /// </summary>
    public class UmbracoPickerAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ProcessValue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Context == null || Context.PropertyDescriptor == null)
            {
                return Enumerable.Empty<object>();
            }

            var propertyType = Context.PropertyDescriptor.PropertyType;
            var isGenericType = propertyType.IsGenericType;
            var targetType = isGenericType
                                ? propertyType.GenericTypeArguments.First()
                                : propertyType;

            if (Value.IsNullOrEmptyString())
            {
                return EnumerableInvocations.Empty(targetType);
            }

            // Single IPublishedContent 
            IPublishedContent content = Value as IPublishedContent;
            if (content != null)
            {
                return content.As(targetType, Context.Culture);
            }

            // ReSharper disable once PossibleNullReferenceException
            var type = Value.GetType();

            // Multiple IPublishedContent 
            if (type.IsEnumerableOfType(typeof(IPublishedContent)))
            {
                return ((IEnumerable<IPublishedContent>)Value).As(targetType, Context.Culture);
            }

            int[] nodeIds = { };

            // First try enumerable strings, ints.
            if (type.IsGenericType || type.IsArray)
            {
                if (type.IsEnumerableOfType(typeof(string)))
                {
                    int n;
                    nodeIds = ((IEnumerable<string>)Value)
                                  .Select(x => int.TryParse(x, NumberStyles.Any, Context.Culture, out n) ? n : -1)
                                  .ToArray();
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = ((IEnumerable<int>)Value).ToArray();
                }
            }

            // Now csv strings.
            if (!nodeIds.Any())
            {
                var s = Value as string ?? Value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    int n;
                    nodeIds = XmlHelper.CouldItBeXml(s)
                    ? s.GetXmlIds()
                    : s
                    .ToDelimitedList()
                    .Select(x => int.TryParse(x, NumberStyles.Any, Context.Culture, out n) ? n : -1)
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

                return multiPicker.As(targetType, Context.Culture);
            }

            return null;
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
