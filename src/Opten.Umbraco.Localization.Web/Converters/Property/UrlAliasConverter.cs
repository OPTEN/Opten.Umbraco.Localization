using Newtonsoft.Json;
using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Core.Models;
using System;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Opten.Umbraco.Localization.Web.Converters.Property
{

#pragma warning disable 1591

	public class UrlAliasConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
	{

		public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
		{
			if (source == null) return null;

			string json = source.ToString();

			if (json.DetectIsJson())
			{
				try
				{
					return JsonConvert.DeserializeObject<UrlAlias[]>(json);
				}
				catch (Exception ex)
				{
					LogHelper.Error<UrlAliasConverter>("Could not parse the json string: " + json, ex);
				}
			}

			return null;
		}

		public override bool IsConverter(PublishedPropertyType propertyType)
		{
			return Opten.Umbraco.Localization.Core.Constants.Conventions.Content.UrlAlias.InvariantEquals(propertyType.PropertyTypeAlias);
		}

		public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
		{
			return PropertyCacheLevel.Content;
		}

		public Type GetPropertyValueType(PublishedPropertyType propertyType)
		{
			return typeof(UrlAlias[]);
		}
	}

#pragma warning restore 1591

}