using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The IPublishedContent Extensions.
	/// </summary>
	public static class IPublishedContentExtensions
	{

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content)
		{
			return content.GetLocalizedTitle(
				recurse: false,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content, string language)
		{
			return content.GetLocalizedTitle(
				recurse: false,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content, bool recurse)
		{
			return content.GetLocalizedTitle(
				recurse: recurse,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content, bool recurse, string language)
		{
			return content.GetLocalizedTitle(
				recurse: recurse,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content, bool recurse, bool withFallback = true)
		{
			return content.GetLocalizedTitle(
				recurse: recurse,
				language: string.Empty,
				withFallback: withFallback);
		}

		/// <summary>
		/// Gets the localized title.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static string GetLocalizedTitle(this IPublishedContent content, bool recurse, string language, bool withFallback = true)
		{
			string title = content.GetLocalizedValue<string>(
				alias: "title",
				recurse: recurse,
				language: language,
				withFallback: withFallback);
			if (string.IsNullOrWhiteSpace(title)) return content.Name;
			else return title;
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content)
		{
			return content.GetLocalizedNavigationName(
				recurse: false,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content, string language)
		{
			return content.GetLocalizedNavigationName(
				recurse: false,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content, bool recurse)
		{
			return content.GetLocalizedNavigationName(
				recurse: recurse,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content, bool recurse, string language)
		{
			return content.GetLocalizedNavigationName(
				recurse: recurse,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content, bool recurse, bool withFallback = true)
		{
			return content.GetLocalizedNavigationName(
				recurse: recurse,
				language: string.Empty,
				withFallback: withFallback);
		}

		/// <summary>
		/// Gets the localized navigation name.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static string GetLocalizedNavigationName(this IPublishedContent content, bool recurse, string language, bool withFallback = true)
		{
			string title = content.GetLocalizedValue<string>(
				alias: "navigationName",
				recurse: recurse,
				language: language,
				withFallback: withFallback);
			if (string.IsNullOrWhiteSpace(title)) return content.GetLocalizedTitle();
			else return title;
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				recurse: false,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias, string language)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				recurse: false,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias, bool recurse)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				recurse: recurse,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias, bool recurse, string language)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				recurse: recurse,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias, bool recurse, bool withFallback = true)
		{
			return content.GetLocalizedValue<T>(
				alias: alias,
				recurse: recurse,
				language: string.Empty,
				withFallback: withFallback);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static T GetLocalizedValue<T>(this IPublishedContent content, string alias, bool recurse, string language, bool withFallback = true)
		{
			Mandate.ParameterNotNull(content, "content");

			var property = content.GetLocalizedProperty(alias, language, recurse, withFallback);

			return property != null ? property.GetValue<T>() : default(T);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias)
		{
			return content.GetLocalizedValue<object>(alias);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias, string language)
		{
			return content.GetLocalizedValue<object>(alias, language);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias, bool recurse)
		{
			return content.GetLocalizedValue<object>(alias, recurse);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias, bool recurse, string language)
		{
			return content.GetLocalizedValue<object>(alias, recurse, language);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias, bool recurse, bool withFallback = true)
		{
			return content.GetLocalizedValue<object>(alias, recurse, withFallback);
		}

		/// <summary>
		/// Gets the localized value.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static object GetLocalizedValue(this IPublishedContent content, string alias, bool recurse, string language, bool withFallback = true)
		{
			return content.GetLocalizedValue<object>(alias, recurse, language, withFallback);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias)
		{
			return content.HasLocalizedValue(
				alias: alias,
				recurse: false,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias, string language)
		{
			return content.HasLocalizedValue(
				alias: alias,
				recurse: false,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias, bool recurse)
		{
			return content.HasLocalizedValue(
				alias: alias,
				recurse: recurse,
				language: string.Empty,
				withFallback: true);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias, bool recurse, string language)
		{
			return content.HasLocalizedValue(
				alias: alias,
				recurse: recurse,
				language: language,
				withFallback: true);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias, bool recurse, bool withFallback = true)
		{
			return content.HasLocalizedValue(
				alias: alias,
				recurse: recurse,
				language: string.Empty,
				withFallback: withFallback);
		}

		/// <summary>
		/// Determines whether [has localized value].
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="language">The language.</param>
		/// <param name="withFallback">if set to <c>false</c> [no fallback to default language].</param>
		/// <returns></returns>
		public static bool HasLocalizedValue(this IPublishedContent content, string alias, bool recurse, string language, bool withFallback = true)
		{
			Mandate.ParameterNotNull(content, "content");

			var prop = GetLocalizedProperty(content, alias, language, recurse, withFallback);
			return prop != null && prop.HasValue;
		}

		/// <summary>
		/// Gets the localized URL for the current UI culture.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		public static string GetLocalizedUrl(this IPublishedContent content)
		{
			System.Web.HttpContext httpContext = System.Web.HttpContext.Current; //TODO: Better way?

			Routing.LocalizedRouting router = new Routing.LocalizedRouting(
				applicationContext: ApplicationContext.Current, //TODO: Better way?
				umbracoContext: UmbracoContext.Current); //TODO: Better way?

			CultureInfo culture = router.TryGetCultureByRequest(
				request: new System.Web.HttpRequestWrapper(httpContext.Request));

			return content.GetLocalizedUrl(language: culture.GetUrlLanguage());
		}

		/// <summary>
		/// Gets the localized URL.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetLocalizedUrl(this IPublishedContent content, string language)
		{
			//TODO: Check if language is valid?

			System.Web.HttpContext httpContext = System.Web.HttpContext.Current;  //TODO: Better way?
			Uri current = httpContext.Request.Url;  //TODO: Better way?

			if (Routing.AliasUrlProvider.FindByUrlAliasEnabled == false)
			{
				//TODO: Should we add also here the "just" replacing way if alias url provider disabled? /de/page => /en/page?
				//TODO: Helper for adding url something like => current.AddUrl("/de/"); with handles the ensure thingy

				// Sometimes the URL contains the host (when hostnames are added) e.g. http://www.opten.ch/de
				// So the content.Url is already an absolute URL => no need the get the base url...
				Uri uri;
				bool withDomain = true;
				if (Uri.TryCreate(content.Url, UriKind.Absolute, out uri) == false)
				{
					// but we have to handle it, if there are no hostnames
					uri = new Uri(current.GetBaseUrl() + content.Url.EnsureStartsWith('/'));
					withDomain = false; // If we don't have any hostnames we don't want to return the domain
				}

				return uri.GetUrlWithLanguage(language: language, withDomain: withDomain);
			}

			Opten.Umbraco.Localization.Web.Routing.AliasUrlProvider urlProvider
				= new Routing.AliasUrlProvider();

			CultureInfo culture = LocalizationContext.TryGetCultureFromUrlLanguage(
				languageName: language);

			return urlProvider.GetUrl(
				umbracoContext: UmbracoContext.Current,
				id: content.Id,
				current: current,
				mode: global::Umbraco.Web.Routing.UrlProviderMode.AutoLegacy,
				culture: culture);
		}

		/// <summary>
		/// Gets the localized property.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="language">The language.</param>
		/// <param name="recurse">if set to <c>true</c> [recurse].</param>
		/// <param name="withFallback">if set to <c>true</c> [with fallback].</param>
		/// <returns></returns>
		public static IPublishedProperty GetLocalizedProperty(this IPublishedContent content, string alias, string language, bool recurse, bool withFallback)
		{
			//TODO: We look up the default culture twice if the value is null, so we could improve that!
			// e.g. alias_de => empty => next try with default culture so alias_de (again) => empty => next try alias

			Mandate.ParameterNotNull(content, "content");

			if (string.IsNullOrWhiteSpace(language))
				language = PropertyHelper.TryGetUrlLanguage();

			string lookup = PropertyHelper.GetAlias(
				alias: alias,
				language: language);

			if (content.HasValue(alias: lookup, recurse: recurse))
			{
				return content.GetProperty(
					alias: lookup,
					recurse: recurse); // Try to get current language => title_en
			}
			else if (withFallback)
			{
				lookup = PropertyHelper.GetAlias(
					alias: alias,
					language: LocalizationContext.DefaultCulture.GetUrlLanguage());

				if (content.HasValue(alias: lookup, recurse: recurse))
				{
					return content.GetProperty(
						alias: lookup,
						recurse: recurse); // Try to get default language => title_de
				}
				else
				{
					lookup = alias;

					return content.GetProperty(
						alias: lookup,
						recurse: recurse); // Try to get without language => title
				}
			}
			else
			{
				return null;
			}
		}

	}
}