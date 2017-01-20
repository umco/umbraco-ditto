using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

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
            if (this.Context == null || this.Context.PropertyDescriptor == null || this.Value.IsNullOrEmptyString())
            {
                return Enumerable.Empty<object>();
            }

            // Single IPublishedContent
            IPublishedContent content = this.Value as IPublishedContent;
            if (content != null)
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
                    int n;
                    nodeIds = ((IEnumerable<string>)this.Value)
                                .Select(x => int.TryParse(x, NumberStyles.Any, this.Context.Culture, out n) ? n : -1)
                                .ToArray();
                }

                if (type.IsEnumerableOfType(typeof(int)))
                {
                    nodeIds = ((IEnumerable<int>)this.Value).ToArray();
                }
            }

            // Now csv strings.
            if (!nodeIds.Any())
            {
                var s = this.Value as string ?? this.Value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    int n;
                    nodeIds = XmlHelper.CouldItBeXml(s)
                    ? s.GetXmlIds()
                    : s.ToDelimitedList()
                        .Select(x => int.TryParse(x, NumberStyles.Any, this.Context.Culture, out n) ? n : -1)
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
                    var item = this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Document, Umbraco.ContentCache.GetById)
                            ?? this.GetPublishedContent(nodeId, ref objectType, UmbracoObjectTypes.Media, Umbraco.MediaCache.GetById)
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

    /// <summary>
    /// A helper for UmbracoPicker processor.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
    internal static class UmbracoPickerHelper
    {
        /// <summary>
        /// Gets the MembershipHelper for the UmbracoPicker processor.
        /// </summary>
        internal static Func<UmbracoContext, MembershipHelper> GetMembershipHelper = (ctx) => UmbracoContext.Current != null
            ? new MembershipHelper(ctx)
            : null;
    }
}