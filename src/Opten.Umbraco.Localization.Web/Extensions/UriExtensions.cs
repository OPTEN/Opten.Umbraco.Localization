using Opten.Umbraco.Localization.Web;
using Opten.Umbraco.Localization.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Opten.Common.Extensions // Add them to the global extensions
{
	/// <summary>
	/// The URI Extensions.
	/// </summary>
	public static class UriExtensions
	{

		//TODO: Helper to get the real other url!

		/// <summary>
		/// Determines whether this uri contains a language.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public static bool ContainsLanguage(this Uri uri)
		{
			IEnumerable<string> toCompare = LocalizationContext.Cultures.Select(o => o.GetUrlLanguage());

			return uri.ContainsSegment(segments: toCompare);
		}

		//TODO: Helper to get the real other url!

		/// <summary>
		/// Determines whether this uri contains a inverted language.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public static bool ContainsInvertedLanguage(this Uri uri)
		{
			IEnumerable<string> toCompare = LocalizationContext.Cultures.Select(o => o.GetUrlLanguage(!LocalizationContext.FullCulture));

			return uri.ContainsSegment(segments: toCompare);
		}

		/// <summary>
		/// Gets the URL with desired language.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="language">The language.</param>
		/// <param name="withQuery">if set to <c>true</c> [with query].</param>
		/// <param name="withDomain">if set to <c>true</c> [with domain].</param>
		/// <returns></returns>
		public static string GetUrlWithLanguage(this Uri uri, string language, bool withQuery = true, bool withDomain = true)
		{
			if ((uri.AbsolutePath + "/").Contains("/" + language + "/"))
				return uri.GetUrl(withQuery: withQuery, withDomain: withDomain);

			string url = uri.GetUrlWithoutLanguage(
				withQuery: withQuery,
				withDomain: false).RemoveLeadingAndTrailingSlash();

			url = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", language, url);

			return ((withDomain) ? uri.GetBaseUrl() : string.Empty) + url;
		}

		/// <summary>
		/// Gets the URL without language.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="withQuery">if set to <c>true</c> [with query].</param>
		/// <param name="withDomain">if set to <c>true</c> [with domain].</param>
		/// <returns></returns>
		public static string GetUrlWithoutLanguage(this Uri uri, bool withQuery = true, bool withDomain = true)
		{
			string url = uri.GetUrl(withQuery: withQuery, withDomain: false);
			if (uri.ContainsLanguage() == false) return url;

			List<string> segments = new List<string>();

			bool found = false;
			string compare;
			foreach (string segment in uri.Segments)
			{
				found = false;
				compare = segment.DecodeSegment();

				if (string.IsNullOrWhiteSpace(compare)) continue;

				foreach (CultureInfo ci in LocalizationContext.Cultures)
					if (compare.Equals(ci.GetUrlLanguage(), StringComparison.OrdinalIgnoreCase))
					{
						found = true;
						break;
					}

				if (found == false) segments.Add(compare);
			}

			string query = url.Contains("?") ? "?" + url.Split('?')[1] : string.Empty;

			string path = segments.Any() ? string.Join("/", segments) : "/"; // TODO: Is "/" a good thing? (equal to uri.AbsolutePath in Opten.Common.Extensions)
			url = string.Format(CultureInfo.InvariantCulture, "{0}{1}", path, query);

			return ((withDomain) ? uri.GetBaseUrl() : string.Empty) + url;
		}


	}
}