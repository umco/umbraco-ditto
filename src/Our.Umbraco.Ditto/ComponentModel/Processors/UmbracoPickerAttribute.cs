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
    /// A picker processor for handling the various types of Umbraco pickers
    /// </summary>
    public class UmbracoPickerAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this.Context == null || this.Context.PropertyInfo == null || this.Value.IsNullOrEmptyString())
            {
                return Enumerable.Empty<IPublishedContent>();
            }

            // Single IPublishedContent
            if (this.Value is IPublishedContent content)
            {
                return content;
            }

            // ReSharper disable once PossibleNullReferenceException
            var type = this.Value.GetType();

            // Multiple IPublishedContent
            if (type.IsEnumerableOfType(typeof(IPublishedContent)))
            {
                return (IEnumerable<IPublishedContent>)this.Value;
            }

            int[] nodeIds = { };

            // First try enumerable strings, ints.
            if (type.IsGenericType || type.IsArray)
            {
                if (type.IsEnumerableOfType(typeof(string)))
                {
                    nodeIds = ((IEnumerable<string>)this.Value)
                        .Select(x => int.TryParse(x, NumberStyles.Any, this.Context.Culture, out int n) ? n : -1)
                        .ToArray();
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = ((IEnumerable<int>)this.Value).ToArray();
                }
            }

            // Now CSV strings.
            if (!nodeIds.Any())
            {
                var s = this.Value as string ?? this.Value.ToString();
                if (string.IsNullOrWhiteSpace(s) == false)
                {
                    nodeIds = XmlHelper.CouldItBeXml(s)
                    ? umbraco.uQuery.GetXmlIds(s)
                    : s.ToDelimitedList()
                        .Select(x => int.TryParse(x, NumberStyles.Any, this.Context.Culture, out int n) ? n : -1)
                        .Where(x => x > 0)
                        .ToArray();
                }
            }

            if (nodeIds.Any())
            {
                var objectType = UmbracoObjectTypes.Unknown;
                var multiPicker = new List<IPublishedContent>();

                // Oh so ugly if you let Resharper do this.
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var nodeId in nodeIds)
                {
                    var item = this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Document, UmbracoContext.ContentCache.GetById)
                            ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Media, UmbracoContext.MediaCache.GetById)
                            ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Member, Members.GetById);

                    if (item != null)
                    {
                        multiPicker.Add(item);
                    }
                }

                return multiPicker;
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