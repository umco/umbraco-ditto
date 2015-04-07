namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Ditto base converter containing reusable properties and converting methods for <see cref="IPublishedContent"/> instances.
    /// All other Ditto converters should inherit from this class.
    /// </summary>
    public abstract class DittoBaseConverter : TypeConverter
    {
        /// <summary>
        /// Gets the <see cref="UmbracoHelper"/> for querying published content or media.
        /// </summary>
        public static UmbracoHelper UmbracoHelper
        {
            get
            {
                // Pull the item from the cache if possible to reduce the db access overhead caused by 
                // multiple reflection iterations for the given type taking place in a single request.
                return (UmbracoHelper)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                        "Ditto.UmbracoHelper",
                        () => new UmbracoHelper(UmbracoContext.Current));
            }
        }

        /// <summary>
        /// Takes a content node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The content node ID.</param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.</param>
        /// <param name="culture">The <see cref="CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        public static object ConvertContentFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            return UmbracoHelper.TypedContent(id).As(targetType, null, null, culture);
        }

        /// <summary>
        /// Takes a media node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The media node ID.</param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.</param>
        /// <param name="culture">The <see cref="CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        public static object ConvertMediaFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            var media = UmbracoHelper.TypedMedia(id);

            // Ensure we are actually returning a media file.
            if (media.HasProperty(Constants.Conventions.Media.File))
            {
                return media.As(targetType, null, null, culture);
            }

            // It's most likely a folder, try its children.
            // This returns an IEnumerable<T>
            return media.Children().As(targetType, null, null, null, culture);
        }

        /// <summary>
        /// Takes a member node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The member node ID.</param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.</param>
        /// <param name="culture">The <see cref="CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        public static object ConvertMemberFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            var umbracoHelper = ConverterHelper.UmbracoHelper;
            return umbracoHelper.TypedMember(id).As(targetType, null, null, culture);
        }

        /// <summary>
        /// Gets Ids from known XML fragments (as saved by the MNTP / XPath CheckBoxList)
        /// </summary>
        /// <param name="xml">The Xml</param>
        /// <returns>An array of node ids as integer.</returns>
        public static int[] GetXmlIds(string xml)
        {
            var ids = new List<int>();

            if (!string.IsNullOrEmpty(xml))
            {
                using (var xmlReader = XmlReader.Create(new StringReader(xml)))
                {
                    try
                    {
                        xmlReader.Read();

                        // Check name of first element
                        switch (xmlReader.Name)
                        {
                            case "MultiNodePicker":
                            case "XPathCheckBoxList":
                            case "CheckBoxTree":

                                // Position on first <nodeId>
                                xmlReader.ReadStartElement();

                                while (!xmlReader.EOF)
                                {
                                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "nodeId")
                                    {
                                        int id;
                                        if (int.TryParse(xmlReader.ReadElementContentAsString(), out id))
                                        {
                                            ids.Add(id);
                                        }
                                    }
                                    else
                                    {
                                        // Step the reader on
                                        xmlReader.Read();
                                    }
                                }

                                break;
                        }
                    }
                    catch
                    {
                        // Failed to read as Xml
                    }
                }
            }

            return ids.ToArray();
        }
    }
}
