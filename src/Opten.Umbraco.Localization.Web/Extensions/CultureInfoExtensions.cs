using System.Globalization;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The CultureInfo Extensions.
	/// </summary>
	public static class CultureInfoExtensions
	{

		/// <summary>
		/// Gets the URL language by the full culture setting.
		/// </summary>
		/// <param name="cultureInfo">The culture information.</param>
		/// <param name="fullCulture">if set to <c>true</c> [full culture].</param>
		/// <returns></returns>
		public static string GetUrlLanguage(this CultureInfo cultureInfo, bool fullCulture)
		{
			return fullCulture ? cultureInfo.Name : cultureInfo.TwoLetterISOLanguageName;
		}

		/// <summary>
		/// Gets the default URL language.
		/// </summary>
		/// <param name="cultureInfo">The culture information.</param>
		/// <returns></returns>
		public static string GetUrlLanguage(this CultureInfo cultureInfo)
		{
			return GetUrlLanguage(cultureInfo, LocalizationContext.FullCulture);
		}

	}
}