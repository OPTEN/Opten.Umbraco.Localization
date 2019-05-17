using Opten.Umbraco.Localization.Web.Extensions;
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
			return GetNotLocalizedAlias(alias) + "_" + GetLanguageAlias(language);
		}

		/// <summary>
		/// Gets the correct localized alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static string GetCorrectLocalizedAlias(string alias)
		{
			CultureInfo cultureInfo = GetCultureByAlias(alias);
			return GetAlias(alias, cultureInfo.GetUrlLanguage());
		}

		/// <summary>
		/// Gets the language alias.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLanguageAlias(string language)
		{
			return language.Replace("-", string.Empty);
		}

		/// <summary>
		/// Tries to get the two letter ISO language.
		/// </summary>
		/// <returns></returns>
		public static string TryGetUrlLanguage()
		{
			string languageName = CultureInfo.CurrentUICulture.GetUrlLanguage();

			if (string.IsNullOrWhiteSpace(languageName))
				languageName = LocalizationContext.DefaultCulture.GetUrlLanguage();

			return languageName;
		}

		/// <summary>
		/// Checks if alias is wrong localized (eg. _en instead of _enUS)
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static bool IsWrongLocalizedProperty(string alias)
		{
			if (IsLocalizedProperty(alias))
			{
				CultureInfo cultureInfo = GetCultureByAlias(alias);
				return alias.Equals(GetAlias(alias, cultureInfo.GetUrlLanguage()), System.StringComparison.OrdinalIgnoreCase) == false;
			}
			return false;
		}

		/// <summary>
		/// Checks if alias is localized (eg. _en)
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static bool IsLocalizedProperty(string alias)
		{
			return Regex.IsMatch(alias, "[a-z]_[a-zA-Z]{2,4}$", RegexOptions.Compiled);
		}

		/// <summary>
		/// Gets clean alias without language
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetNotLocalizedAlias(string alias)
		{
			return Regex.Replace(alias, "_[a-zA-Z]{2,4}$", string.Empty, RegexOptions.Compiled);
		}

		/// <summary>
		/// Gets the culture by the alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static CultureInfo GetCultureByAlias(string alias)
		{
			var match = Regex.Match(alias, "_([a-zA-Z]{2,4})$", RegexOptions.Compiled);
			var language = string.Empty;
			if (match.Success)
			{
				language = match.Groups[1].ToString();
				if (language.Length > 2)
				{
					language = language.Insert(2, "-");
				}
			}
			return LocalizationContext.TryGetCultureFromUrlLanguage(language);
		}

		/// <summary>
		/// Gets inverted localized alias
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		public static string GetInvertedLocalizedAlias(string alias, CultureInfo cultureInfo)
		{
			return GetAlias(alias, cultureInfo.GetUrlLanguage(!LocalizationContext.FullCulture));
		}

	}
}