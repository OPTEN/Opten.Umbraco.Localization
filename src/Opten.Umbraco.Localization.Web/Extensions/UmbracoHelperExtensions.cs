using Opten.Umbraco.Localization.Web.Helpers;
using Umbraco.Web;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The UmbracoHelper Extensions.
	/// </summary>
	public static class UmbracoHelperExtensions
	{
		/// <summary>
		/// Gets the dictionary value with fallback to different cultures.
		/// </summary>
		/// <param name="umbracoHelper">The umbraco helper.</param>
		/// <param name="key">The key of the dictionary.</param>
		/// <param name="defaultValue">The default value to show if no dictionary could be found.</param>
		/// <param name="languageName">Name of the language, if empty it takes the current culture.</param>
		/// <returns></returns>
		public static string GetLocalizedDictionaryValue(this UmbracoHelper umbracoHelper, string key, string defaultValue = "", string languageName = "")
		{
			var dictionary = umbracoHelper.GetDictionaryValue(key);
			if (string.IsNullOrWhiteSpace(dictionary) == false)
			{
				return dictionary;
			}

			return DictionaryHelper.GetDictionaryValue(key, defaultValue, languageName);
		}
	}
}
