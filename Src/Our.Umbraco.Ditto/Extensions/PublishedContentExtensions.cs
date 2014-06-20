using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
	public static class PublishedContentExtensions
	{
		public static T As<T>(this IPublishedContent content,
			Action<ConvertingTypeEventArgs> convertingType = null,
			Action<ConvertedTypeEventArgs> convertedType = null)
			where T : class
		{
			if (content == null)
				return default(T);

			using (var t = DisposableTimer.DebugDuration<T>(string.Format("IPublishedContent As ({0})", content.DocumentTypeAlias)))
			{
				var type = typeof(T);
				T instance;

				var constructor = type.GetConstructors()
					.OrderBy(x => x.GetParameters().Length)
					.First();
				var constructorParams = constructor.GetParameters();

				var args1 = new ConvertingTypeEventArgs { Content = content };

				EventHandlers.CallConvertingTypeHandler(args1);

				if (!args1.Cancel && convertingType != null)
					convertingType(args1);

				if (args1.Cancel)
					return default(T);

				if (constructorParams.Length == 0)
				{
					instance = Activator.CreateInstance(type) as T;
				}
				else if (constructorParams.Length == 1 & constructorParams[0].ParameterType == typeof(IPublishedContent))
				{
					instance = Activator.CreateInstance(type, content) as T;
				}
				else
				{
					throw new InvalidOperationException("Type {0} has invalid constructor parameters");
				}

				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				var contentType = content.GetType();

				foreach (var propertyInfo in properties.Where(x => x.CanWrite))
				{
					using (var propertyLoopTimer = DisposableTimer.DebugDuration<T>(string.Format("ForEach Property ({1} {0})", propertyInfo.Name, content.Id)))
					{
						var umbracoPropertyName = propertyInfo.Name;
						var altUmbracoPropertyName = string.Empty;
						var recursive = false;
						object defaultValue = null;

						var umbracoPropertyAttr = propertyInfo.GetCustomAttribute<UmbracoPropertyAttribute>();
						if (umbracoPropertyAttr != null)
						{
							umbracoPropertyName = umbracoPropertyAttr.PropertyName;
							altUmbracoPropertyName = umbracoPropertyAttr.AltPropertyName;
							recursive = umbracoPropertyAttr.Recursive;
							defaultValue = umbracoPropertyAttr.DefaultValue;
						}

						// Try fetching the value
						var contentProperty = contentType.GetProperty(umbracoPropertyName);
						var propertyValue = contentProperty != null
							? contentProperty.GetValue(content, null)
							: content.GetPropertyValue(umbracoPropertyName,
								recursive);

						// Try fetching the alt value
						if (propertyValue == null && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
						{
							contentProperty = contentType.GetProperty(altUmbracoPropertyName);
							propertyValue = contentProperty != null
								? contentProperty.GetValue(content, null)
								: content.GetPropertyValue(altUmbracoPropertyName,
									recursive);
						}

						// Try setting the default value
						if (propertyValue == null && defaultValue != null)
							propertyValue = defaultValue;

						// Process the value
						if (propertyValue != null)
						{
							if (propertyInfo.PropertyType.IsInstanceOfType(propertyValue))
							{
								propertyInfo.SetValue(instance, propertyValue, null);
							}
							else
							{
								using (var typeConverterTimer = DisposableTimer.DebugDuration<T>(string.Format("TypeConverter ({1} {0})", propertyInfo.Name, content.Id)))
								{
									var converterAttr = propertyInfo.GetCustomAttribute<TypeConverterAttribute>();
									if (converterAttr != null)
									{
										var converter = Activator.CreateInstance(Type.GetType(converterAttr.ConverterTypeName)) as TypeConverter;
										propertyInfo.SetValue(instance, converter.ConvertFrom(propertyValue), null);
									}
									else
									{
										var convert = propertyValue.TryConvertTo(propertyInfo.PropertyType);
										if (convert.Success)
										{
											propertyInfo.SetValue(instance, convert.Result, null);
										}
									}
								}
							}
						}
					}
				}

				var args2 = new ConvertedTypeEventArgs
				{
					Content = content,
					Converted = instance,
					ConvertedType = typeof(T)
				};

				if (convertedType != null)
					convertedType(args2);

				EventHandlers.CallConvertedTypeHandler(args2);

				return args2.Converted as T;
			}
		}

		public static IEnumerable<T> As<T>(this IEnumerable<IPublishedContent> items,
			string documentTypeAlias = null,
			Action<ConvertingTypeEventArgs> convertingType = null,
			Action<ConvertedTypeEventArgs> convertedType = null)
			where T : class
		{
			using (var t = DisposableTimer.DebugDuration<IEnumerable<T>>(string.Format("As ({0})", documentTypeAlias)))
			{
				if (string.IsNullOrWhiteSpace(documentTypeAlias))
					return items.Select(x => x.As<T>(convertingType, convertedType));

				return items
					.Where(x => documentTypeAlias.InvariantEquals(x.DocumentTypeAlias))
					.Select(x => x.As<T>(convertingType, convertedType));
			}
		}
	}
}