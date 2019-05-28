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
		/// Gets the localized dictionary value.
		/// </summary>
		/// <param name="umbracoHelper">The umbraco helper.</param>
		/// <param name="dictionaryKey">The dictionary key.</param>
		/// <param name="languageName">Name of the language.</param>
		/// <returns></returns>
		public static string GetLocalizedDictionaryValue(this UmbracoHelper umbracoHelper, string dictionaryKey, string languageName = "")
		{
			var dictionary = umbracoHelper.GetDictionaryValue(dictionaryKey);
			if (string.IsNullOrWhiteSpace(dictionary) == false)
			{
				return dictionary;
			}

			return DictionaryHelper.GetDictionaryValue(dictionaryKey, languageName);
		}
	}
}
