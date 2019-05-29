using Opten.Umbraco.Localization.Web.Extensions;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Opten.Umbraco.Localization.Web.Helpers
{
	/// <summary>
	/// The Dictionary Helper.
	/// </summary>
	public static class DictionaryHelper
	{
		private static ILocalizationService _localizationService = ApplicationContext.Current.Services.LocalizationService;

		/// <summary>
		/// Gets the dictionary value with fallback to different cultures.
		/// </summary>
		/// <param name="key">The key of the dictionary.</param>
		/// <param name="defaultValue">The default value to show if no dictionary could be found.</param>
		/// <param name="languageName">Name of the language, if empty it takes the current culture.</param>
		/// <returns></returns>
		public static string GetDictionaryValue(string key, string defaultValue = "", string languageName = "")
		{
			if (string.IsNullOrWhiteSpace(languageName))
			{
				languageName = LocalizationContext.GetDefaultLanguage(LocalizationContext.Languages).CultureInfo.GetUrlLanguage();
			}
			var dictionaryItem = _localizationService.GetDictionaryItemByKey(key);
			if (dictionaryItem != null)
			{
				var dictionary = GetTranslation(dictionaryItem, languageName);
				if (string.IsNullOrWhiteSpace(dictionary) == false)
				{
					return dictionary;
				}
				var fallbackCulture = LocalizationContext.FallbackCulture(languageName);
				var defaultCulture = LocalizationContext.DefaultCulture;
				while (fallbackCulture != defaultCulture)
				{
					dictionary = GetTranslation(dictionaryItem, fallbackCulture.GetUrlLanguage());
					if (string.IsNullOrWhiteSpace(dictionary) == false)
					{
						return dictionary;
					}
					fallbackCulture = LocalizationContext.FallbackCulture(fallbackCulture.GetUrlLanguage());
				}
				dictionary = GetTranslation(dictionaryItem, defaultCulture.GetUrlLanguage());
				if (string.IsNullOrWhiteSpace(dictionary) == false)
				{
					return dictionary;
				}
			}
			return defaultValue;
		}

		private static string GetTranslation(IDictionaryItem dictionaryItem, string languageName)
		{
			var translation = dictionaryItem.Translations.SingleOrDefault(x => x.Language.CultureInfo.GetUrlLanguage().Equals(languageName, System.StringComparison.OrdinalIgnoreCase));
			if (translation != null)
			{
				return translation.Value;
			}
			return null;
		}

	}
}
