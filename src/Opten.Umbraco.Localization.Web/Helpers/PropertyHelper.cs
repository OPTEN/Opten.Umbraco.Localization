using System.Globalization;
using System.Text.RegularExpressions;

namespace Opten.Umbraco.Localization.Web.Helpers
{
	/// <summary>
	/// The Property Helper.
	/// </summary>
	public static class PropertyHelper
	{

		/// <summary>
		/// Gets the alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetAlias(string alias, string language)
		{
			return alias + "_" + language.ToLowerInvariant();
		}

		/// <summary>
		/// Tries to get the two letter ISO language.
		/// </summary>
		/// <returns></returns>
		public static string TryGetTwoLetterISOLanguageName()
		{
			string twoLetterISOCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

			if (string.IsNullOrWhiteSpace(twoLetterISOCode))
				twoLetterISOCode = LocalizationContext.DefaultCulture.TwoLetterISOLanguageName;

			return twoLetterISOCode;
		}

		/// <summary>
		/// Checks if alias is localized (eg. _en)
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static bool IsLocalizedProperty(string alias)
		{
			return Regex.IsMatch(alias, "[a-z]_[a-z]{2}$", RegexOptions.Compiled);
		}

		/// <summary>
		/// Gets clean alias without language
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetNotLocalizedAlias(string alias)
		{
			return Regex.Replace(alias, "_[a-z]{2}$", string.Empty, RegexOptions.Compiled);
		}

	}
}