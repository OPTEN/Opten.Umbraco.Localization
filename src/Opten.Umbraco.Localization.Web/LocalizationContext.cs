using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Controllers;
using Opten.Umbraco.Localization.Web.Extensions;
using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
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
		/// Gets the fallback culture.
		/// </summary>
		/// <param name="languageName">Name of the language.</param>
		/// <returns></returns>
		public static CultureInfo FallbackCulture(string languageName)
		{
			// Try to get the fallback culture by the web.config
			string fallbackCultureName = ConfigurationManager.AppSettings.Get<string>(key: string.Format("OPTEN:localization:fallback:{0}", languageName));

			if (string.IsNullOrWhiteSpace(fallbackCultureName) == false)
			{
				var langauge = GetActiveLanguage(Languages, fallbackCultureName);
				if (langauge != null)
				{
					return langauge.CultureInfo;
				}
			}
			return DefaultCulture;
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
		/// Gets a value indicating whether to use full culture or not.
		/// </summary>
		/// <value>
		///   <c>true</c> if [full culture]; otherwise, <c>false</c>.
		/// </value>
		public static bool FullCulture
		{
			get
			{
				bool fullCulture;
				bool hasKey = Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:fullCulture"], out fullCulture);
				return fullCulture;
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
				var defaultBackofficeCultures = ConfigurationManager.AppSettings.Get<string>(key: "OPTEN:localization:defaultBackofficeCultures");
				if (string.IsNullOrWhiteSpace(defaultBackofficeCultures))
				{
					return new[] { GetDefaultLanguage(LocalizationContext.Languages) };
				}
				else
				{
					string[] isoCodes = defaultBackofficeCultures.Replace(" ", string.Empty).Split(',');

					return LocalizationContext.Languages.Where(o => isoCodes.Contains(o.IsoCode, StringComparer.OrdinalIgnoreCase)).ToArray();
				}
			}
			else
			{
				string[] isoCodes = cookie.Value.Split(',');

				return LocalizationContext.Languages.Where(o => isoCodes.Contains(o.IsoCode)).ToArray();
			}
		}

		/// <summary>
		/// Gets the default language.
		/// </summary>
		/// <param name="languages">The languages.</param>
		/// <returns></returns>
		internal static ILanguage GetDefaultLanguage(IEnumerable<ILanguage> languages)
		{
			ILanguage language = null;

			// If not try set from current thread
			if (language == null)
				language = GetActiveLanguage(languages, Thread.CurrentThread.CurrentUICulture.Name);

			// Couldn't find from the thread, so try from web.config
			if (language == null)
				language = GetActiveLanguage(languages, LocalizationContext.DefaultCulture.Name);

			// Couldn't find a good enough match, just select the first language
			if (language == null)
				language = languages.FirstOrDefault();

			return language;
		}

		/// <summary>
		/// Gets the active language.
		/// </summary>
		/// <param name="languages">The languages.</param>
		/// <param name="isoCode">The iso code.</param>
		/// <returns></returns>
		internal static ILanguage GetActiveLanguage(
			IEnumerable<ILanguage> languages,
			string isoCode)
		{
			// Try settings to exact match of current culture
			ILanguage language = languages.FirstOrDefault(o => o.CultureInfo.Name == isoCode);

			// Try setting to nearest match
			if (language == null)
				language = languages.FirstOrDefault(o => o.CultureInfo.Name.Contains(isoCode));

			// Try setting to nearest match
			if (language == null)
				language = languages.FirstOrDefault(o => isoCode.Contains(o.CultureInfo.Name));

			return language;
		}

		/// <summary>
		/// Tries to get the culture from two letter iso code if not found default will returned.
		/// </summary>
		/// <param name="languageName">Name of language.</param>
		/// <returns></returns>
		internal static CultureInfo TryGetCultureFromUrlLanguage(string languageName)
		{
			var culture = Cultures.FirstOrDefault(o => o.GetUrlLanguage().StartsWith(languageName, StringComparison.OrdinalIgnoreCase));
			if (culture != null)
			{
				return culture;
			}
			if (languageName.Contains("-"))
			{
				return TryGetCultureFromUrlLanguage(languageName.Split('-')[0]);
			}
			return DefaultCulture;
		}

		public static RegionInfo GetRegionByCurrentIPAddress()
		{
			RegionInfo currentRegion = new LocationApiControler().GetRegionInfoByCurrentIPAddress();
			if (currentRegion != null)
			{
				return currentRegion;
			}
			return new RegionInfo(DefaultCulture.Name);
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
