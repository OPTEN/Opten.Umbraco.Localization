using Opten.Umbraco.Localization.Web.Extensions;
using System.Linq;
using Umbraco.Core;
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
		/// Gets the dictionary value by the language.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="languageName">Name of the language.</param>
		/// <returns></returns>
		public static string GetDictionaryValue(string key, string languageName = "")
		{
			if (string.IsNullOrWhiteSpace(languageName))
			{
				languageName = LocalizationContext.DefaultCulture.GetUrlLanguage();
			}
			var dictionaryItem = _localizationService.GetDictionaryItemByKey(key);
			if (dictionaryItem != null)
			{
				var translation = dictionaryItem.Translations.SingleOrDefault(x => x.Language.CultureInfo.GetUrlLanguage().Equals(languageName, System.StringComparison.OrdinalIgnoreCase));
				var dictionary = translation.Value;
				if (string.IsNullOrWhiteSpace(dictionary) == false)
				{
					return dictionary;
				}
			}
			return "[" + key + "]";
		}

	}
}
