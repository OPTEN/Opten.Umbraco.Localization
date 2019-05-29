using Opten.Umbraco.Localization.Web.Helpers;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The Content Base Extensions.
	/// </summary>
	public static class ContentBaseExtensions
	{

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IContentBase content, string alias)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IContentBase content, string alias, string language, bool withFallback = true)
		{
			Mandate.ParameterNotNull(content, "content");

			return content.GetLocalizedProperty<T>(alias, language, withFallback);
		}

		/// <summary>
		/// Gets the localized property.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>true</c> [with fallback].</param>
		/// <returns></returns>
		public static T GetLocalizedProperty<T>(this IContentBase content, string alias, string language, bool withFallback)
		{
			//TODO: We look up the default culture twice if the value is null, so we could improve that!
			// e.g. alias_de => empty => next try with default culture so alias_de (again) => empty => next try alias

			Mandate.ParameterNotNull(content, "content");

			if (string.IsNullOrWhiteSpace(language))
				language = PropertyHelper.TryGetUrlLanguage();

			string lookup = PropertyHelper.GetAlias(
				alias: alias,
				language: language);

			if (content.HasProperty(propertyTypeAlias: lookup))
			{
				return content.GetValue<T>(propertyTypeAlias: lookup); // Try to get current language => title_en
			}
			else if (withFallback)
			{
				var fallbackCulture = LocalizationContext.FallbackCulture(language);
				var defaultCulture = LocalizationContext.DefaultCulture;
				while (fallbackCulture != defaultCulture)
				{
					lookup = PropertyHelper.GetAlias(
						alias: alias,
						language: fallbackCulture.GetUrlLanguage());

					if (content.HasProperty(propertyTypeAlias: lookup))
					{
						return content.GetValue<T>(propertyTypeAlias: lookup); // Try to get default language => title_de
					}
					fallbackCulture = LocalizationContext.FallbackCulture(fallbackCulture.GetUrlLanguage());
				}

				lookup = PropertyHelper.GetAlias(
					alias: alias,
					language: defaultCulture.GetUrlLanguage());

				if (content.HasProperty(propertyTypeAlias: lookup))
				{
					return content.GetValue<T>(propertyTypeAlias: lookup); // Try to get default language => title_de
				}
				else
				{
					lookup = alias;

					return content.GetValue<T>(propertyTypeAlias: lookup); // Try to get without language => title
				}
			}
			else
			{
				return default(T);
			}
		}
	}
}
