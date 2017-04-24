using Opten.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Opten.Umbraco.Localization.Web
{
	/// <summary>
	/// The Application's Localization Context.
	/// </summary>
	public static class LocalizationContext
	{

		//TODO: Non static? + Events when creating language to remove cache

		internal static CultureInfo[] _cultures { get; set; } //TODO: Internal because for testing => has to be done better umb 7.3!

		/// <summary>
		/// Gets the installed cultures.
		/// </summary>
		/// <value>
		/// The cultures.
		/// </value>
		public static CultureInfo[] Cultures
		{
			get
			{
				if (_cultures == null)
				{
					List<CultureInfo> cultures = new List<CultureInfo>();

					foreach (ILanguage language in Languages)
					{
						cultures.Add(language.CultureInfo);
					}

					_cultures = cultures.ToArray();
				}

				return _cultures;
			}
		}

		/// <summary>
		/// Gets the installed languages.
		/// </summary>
		/// <value>
		/// The languages.
		/// </value>
		public static ILanguage[] Languages
		{
			get
			{
				return GetLanguages(applicationContext: ApplicationContext.Current);
			}
		}

		/// <summary>
		/// Gets the default culture.
		/// </summary>
		/// <value>
		/// The default culture.
		/// </value>
		public static CultureInfo DefaultCulture
		{
			get
			{
				// Try to get the default culture by the web.config
				string defaultCulture = ConfigurationManager.AppSettings.Get<string>(key: "OPTEN:localization:defaultCulture");

				// If not set we return the first language installed in umbraco
				if (string.IsNullOrWhiteSpace(defaultCulture))
					return Cultures.First();

				CultureInfo cultureInfo = Cultures.FirstOrDefault(o =>
					o.TwoLetterISOLanguageName.Equals(defaultCulture, StringComparison.OrdinalIgnoreCase) ||
					o.Name.Equals(defaultCulture, StringComparison.OrdinalIgnoreCase));

				if (cultureInfo == null)
				{
					throw new CultureNotFoundException("Culture with two letter ISO code or name '" + defaultCulture + "'"
					+ " not found! Maybe you spelled something wrong in the web.config?");
				}

				return cultureInfo;
			}
		}

		/// <summary>
		/// Gets the current back end user languages BUT ONLY when a back end request was accomplished.
		/// </summary>
		/// <returns></returns>
		public static ILanguage[] CurrentBackEndUserLanguages()
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[Core.Constants.Cache.BackendLanguages];

			if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
			{
				//TODO: This is kind of wrong because it should take the default language which
				//		is selected in LocalizationApiController.GetAllLanguages()
				return LocalizationContext.Languages;
			}
			else
			{
				string[] isoCodes = cookie.Value.Split(',');

				return LocalizationContext.Languages.Where(o => isoCodes.Contains(o.IsoCode)).ToArray();
			}
		}

		/// <summary>
		/// Determines whether the language is available in the umbraco's backend.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">Name of the two letter iso language.</param>
		/// <returns></returns>
		internal static bool IsValidLanguage(string twoLetterISOLanguageName)
		{
			foreach (CultureInfo ci in Cultures)
				if (twoLetterISOLanguageName.Equals(ci.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
					return true;

			return false;
		}

		/// <summary>
		/// Tries to get the culture from two letter iso code if not found default will returned.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">Name of the two letter iso language.</param>
		/// <returns></returns>
		internal static CultureInfo TryGetCultureFromTwoLetterIsoCode(string twoLetterISOLanguageName)
		{
			if (IsValidLanguage(twoLetterISOLanguageName))
			{
				return Cultures.First(o => o.Name.StartsWith(twoLetterISOLanguageName));
			}
			else
			{
				return DefaultCulture;
			}
		}

		#region Private helpers

		private static ILanguage[] GetLanguages(ApplicationContext applicationContext)
		{
			return applicationContext.ApplicationCache.RuntimeCache.GetCacheItem(
				cacheKey: Core.Constants.Cache.Languages,
				getCacheItem: () =>
				{
					return applicationContext.Services.LocalizationService.GetAllLanguages().ToArray();
				},
				timeout: TimeSpan.FromHours(24)) as ILanguage[];
		}

		#endregion

	}
}