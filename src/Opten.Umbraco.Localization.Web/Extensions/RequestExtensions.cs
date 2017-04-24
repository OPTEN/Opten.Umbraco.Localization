using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Umbraco.Core.Logging;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The HTTP Request Extensions.
	/// </summary>
	public static class RequestExtensions
	{

		/// <summary>
		/// Gets the current culture from the http request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		public static CultureInfo GetCurrentCulture(this HttpRequestBase request)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

			return LocalizationContext.TryGetCultureFromTwoLetterIsoCode(
				twoLetterISOLanguageName: currentCulture.TwoLetterISOLanguageName);
		}

		/// <summary>
		/// Gets the current UI culture from the http request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="getFromBrowserIfNotFound">if set to <c>true</c> [get from browser if not found].</param>
		/// <returns></returns>
		public static CultureInfo GetCurrentUICulture(this HttpRequestBase request, bool getFromBrowserIfNotFound = true)
		{
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;

			// If not found try to get it from the browser
			if (getFromBrowserIfNotFound && currentUICulture == null)
				currentUICulture = request.TryGetCultureFromBrowser();

			return LocalizationContext.TryGetCultureFromTwoLetterIsoCode(
				twoLetterISOLanguageName: currentUICulture.TwoLetterISOLanguageName);
		}

		/// <summary>
		/// Tries to get the culture from browser if not found default language will returned.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>Browser culture or default culture if not found.</returns>
		public static CultureInfo TryGetCultureFromBrowser(this HttpRequestBase request)
		{
			//TODO: Move to Opten.Common?
			CultureInfo defaultCulture = LocalizationContext.DefaultCulture;

			try
			{
				string browserCulture = (request.UserLanguages ?? Enumerable.Empty<string>()).FirstOrDefault();

				if (string.IsNullOrWhiteSpace(browserCulture))
					browserCulture = Thread.CurrentThread.CurrentUICulture.Name;

				return LocalizationContext.TryGetCultureFromTwoLetterIsoCode(
					twoLetterISOLanguageName: browserCulture.GetTwoLetterISOCodeByName());
			}
			catch (Exception ex)
			{
				LogHelper.Error<HttpRequestBase>("Couldn't get browser language -> return default language: " + defaultCulture, ex);
				return defaultCulture;
			}
		}

		#region Private helpers

		private static string GetTwoLetterISOCodeByName(this string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return string.Empty;
			if (name.Contains("-") == false) return name;
			return name.Split('-')[0];
		}

		#endregion

	}
}