using Opten.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	/// <summary>
	/// The Alias URL Provider (Localized URLs).
	/// </summary>
	/// <seealso cref="Umbraco.Web.Routing.IUrlProvider" />
	public class AliasUrlProvider : IUrlProvider
	{

		private readonly RoutingHelper _routingHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="AliasUrlProvider" /> class.
		/// </summary>
		/// <param name="domainService">The domain service.</param>
		public AliasUrlProvider(IDomainService domainService)
		{
			_routingHelper = new RoutingHelper(domainService: domainService);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AliasUrlProvider" /> class.
		/// </summary>
		public AliasUrlProvider() : this(ApplicationContext.Current.Services.DomainService) { }

		/// <summary>
		/// Gets the localized URL by the thread's current ui culture.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="current">The current.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>
		/// The url for the published content.
		/// </returns>
		/// <exception cref="System.ArgumentException">Current url must be absolute.;current</exception>
		/// <remarks>
		/// <para>The url is absolute or relative depending on <c>mode</c> and on <c>current</c>.</para>
		/// <para>If the provider is unable to provide a url, it should return <c>null</c>.</para>
		/// </remarks>
		public string GetUrl(
			UmbracoContext umbracoContext,
			int id,
			Uri current,
			UrlProviderMode mode)
		{
			return this.GetUri(
				umbracoContext: umbracoContext,
				id: id,
				current: current,
				mode: mode,
				cultureInfo: Thread.CurrentThread.CurrentUICulture)?.Url;
		}

		/// <summary>
		/// Gets the localized URI.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="current">The current.</param>
		/// <param name="mode">The mode.</param>
		/// <param name="cultureInfo">The culture.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Current url must be absolute.;current</exception>
		/// <exception cref="ArgumentException">Current url must be absolute. - current</exception>
		public LocalizedUri GetUri(
			UmbracoContext umbracoContext,
			int id,
			Uri current,
			UrlProviderMode mode,
			CultureInfo cultureInfo)
		{
			if (FindByUrlAliasEnabled)
			{
				if (current.IsAbsoluteUri == false)
				{
					throw new ArgumentException("Current url must be absolute.", nameof(current));
				}
				if (cultureInfo == null)
				{
					throw new ArgumentException(nameof(cultureInfo));
				}

				if (umbracoContext.IsFrontEndUmbracoRequest == false)
				{
					// this is just an attempt to display the correct url in the "Info" tab
					// for the current user (e.g. if backend language is fr-CH but only de-CH is on frontend).
					cultureInfo = LocalizationContext.IsValidLanguage(cultureInfo.TwoLetterISOLanguageName)
						? cultureInfo
						: LocalizationContext.DefaultCulture;
				}
				
				LocalizedUri uri = _routingHelper.GetLocalizedUri(umbracoContext, id, cultureInfo);

				if (uri == null || string.IsNullOrWhiteSpace(uri.Route))
				{
					LogHelper.Debug<AliasUrlProvider>(
						"Couldn't find any page with contentId={0}. This is most likely caused by the page not being published.",
						() => id);

					return null;
				}

				// assemble the url from domainUri (maybe null) and path
				uri.Url = _routingHelper.AssembleUrl(
					route: uri.Route,
					current: current,
					mode: mode,
					cultureInfo: cultureInfo);

				return uri;
			}
			else
			{
				// other provider should take care of it.
				return null;
			}
		}

		/// <summary>
		/// Gets the other urls of a published content.
		/// </summary>
		/// <param name="umbracoContext">The Umbraco context.</param>
		/// <param name="id">The published content id.</param>
		/// <param name="current">The current absolute url.</param>
		/// <returns>
		/// The other urls for the published content.
		/// </returns>
		/// <remarks>
		/// Other urls are those that <c>GetUrl</c> would not return in the current context, but would be valid
		/// urls for the node in other contexts (different domain for current request, umbracoUrlAlias...).
		/// </remarks>
		public IEnumerable<string> GetOtherUrls(UmbracoContext umbracoContext, int id, Uri current)
		{
			if (FindByUrlAliasEnabled == false)
			{
				return Enumerable.Empty<string>();
			}

			string route = umbracoContext.ContentCache.GetRouteById(id);

			// extract domainRootId and path
			// route is /<path> or <domainRootId>/<path>
			int pos = route.IndexOf('/');
			string path = pos == 0 ? route : route.Substring(pos);

			int domainRootId = pos == 0 ? 0 : int.Parse(route.Substring(0, pos));

			IEnumerable<DomainAndUri> domainUris = pos == 0
				? null
				: _routingHelper.UmbracoDomainsForNode(domainRootId, current);

			if (domainUris == null || domainUris.Any() == false)
			{
				// if there are no domains (hostnames) we don't have to do anything
				// because we do not have smth. like /de /en, right?!
				return Enumerable.Empty<string>();
			}

			List<string> otherUrls = new List<string>();

			foreach (CultureInfo cultureInfo in LocalizationContext.Cultures)
			{
				LocalizedUri uri = this.GetUri(umbracoContext, id, current, UrlProviderMode.AutoLegacy, cultureInfo);

				if (uri != null && uri.Localized)
				{
					otherUrls.Add(uri.Url);
				}
			}

			return otherUrls
				.OrderBy(o => o)
				.Distinct()
				.ToList();
		}

		/// <summary>
		/// Gets a value indicating whether [find by URL alias enabled].
		/// </summary>
		/// <value>
		/// <c>true</c> if [find by URL alias enabled]; otherwise, <c>false</c>.
		/// </value>
		public static bool FindByUrlAliasEnabled
		{
			get
			{
				// finder 
				if (ContentFinderResolver.Current.ContainsType<Routing.ContentFinderByUrlAlias>())
				{
					return true;
				}

				// handler wrapped into a finder 
				//if (ContentFinderResolver.Current.ContainsType<ContentFinderByNotFoundHandler<global::umbraco.SearchForAlias>>())
				//	return true;

				// anything else, we can't detect 
				return false;
			}
		}

		private string CombinePaths(string path1, string path2)
		{
			string path = path1.TrimEnd('/') + path2;
			return path == "/" ? path : path.TrimEnd('/');
		}

		private IEnumerable<string> AssembleOtherUrls(IEnumerable<DomainAndUri> domainUris, string path)
		{
			// Copied from umbraco
			//TODO Should we use that aswell?
			//if (mode == UrlProviderMode.AutoLegacy)
			//{
			//	mode = _requestSettings.UseDomainPrefixes
			//		? UrlProviderMode.Absolute
			//		: UrlProviderMode.Auto;
			//}

			List<string> otherUrls = new List<string>();

			if (domainUris == null)
			{
				otherUrls.Add(UriUtility.UriFromUmbraco(new Uri(path, UriKind.Relative)).ToString());
			}
			else
			{
				otherUrls.AddRange(domainUris
					.Select(domainUri => new Uri(CombinePaths(domainUri.Uri.GetLeftPart(UriPartial.Path), path)))
					.Select(uri => UriUtility.UriFromUmbraco(uri).ToString()));
			}

			return otherUrls;
		}
	}
}
