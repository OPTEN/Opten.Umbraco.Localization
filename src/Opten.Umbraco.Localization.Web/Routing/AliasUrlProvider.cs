using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	/// <summary>
	/// The Alias URL Provider (Localized URLs).
	/// </summary>
	public class AliasUrlProvider : IUrlProvider
	{

		private readonly RoutingHelper _routingHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="AliasUrlProvider"/> class.
		/// </summary>
		/// <param name="domainService">The domain service.</param>
		public AliasUrlProvider(IDomainService domainService)
		{
			_routingHelper = new RoutingHelper(domainService: domainService);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AliasUrlProvider"/> class.
		/// </summary>
		public AliasUrlProvider() : this(ApplicationContext.Current.Services.DomainService) { }

		/// <summary>
		/// Gets the localized URL by the thread's current ui culture.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="current">The current.</param>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Current url must be absolute.;current</exception>
		public string GetUrl(
			UmbracoContext umbracoContext,
			int id,
			Uri current,
			UrlProviderMode mode)
		{
			return this.GetUrl(
				umbracoContext: umbracoContext,
				id: id,
				current: current,
				mode: mode,
				culture: Thread.CurrentThread.CurrentUICulture);
		}

		/// <summary>
		/// Gets the localized URL.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="current">The current.</param>
		/// <param name="mode">The mode.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Current url must be absolute.;current</exception>
		public string GetUrl(
			UmbracoContext umbracoContext,
			int id,
			Uri current,
			UrlProviderMode mode,
			CultureInfo culture)
		{
			if (FindByUrlAliasEnabled/* && umbracoContext.IsFrontEndUmbracoRequest*/)
			{
				if (current.IsAbsoluteUri == false)
					throw new ArgumentException("Current url must be absolute.", "current");

				string isoCode = culture.Name;

				// will not use cache if previewing
				bool anyLocalizedUrl;
				string route = _routingHelper.GetRouteById(
					umbracoContext,
					umbracoContext.InPreviewMode,
					id,
					isoCode,
					out anyLocalizedUrl);

				// NO! We want to localize it because of Surface/Api Controller!
				//if (anyLocalizedUrl == false) return null; // other provider should take care of it

				if (string.IsNullOrWhiteSpace(route))
				{
					LogHelper.Debug<AliasUrlProvider>(
						"Couldn't find any page with contentId={0}. This is most likely caused by the page not being published.",
						() => id);
					return null;
				}

				// assemble the url from domainUri (maybe null) and path
				return _routingHelper.AssembleUrl(
					route: route,
					current: current,
					mode: mode,
					isoCode: isoCode);
			}
			else
			{
				return null; // Other provider should take care of it.
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

			IPublishedContent content = umbracoContext.ContentCache.GetById(id);

			/*UrlAlias[] urlAlias = _routingHelper.GetUrlAlias(content: content);*/

			bool hasDomains = _routingHelper.NodeHasDomains(
				contentId: content.Id);

			if (hasDomains)
			{
				// If content has domains we do not have to walk up
				// but when not then we have to, because we don't know if somewhere is a localized url
				return Enumerable.Empty<string>();
			}

			IPublishedContent node = content;
			IEnumerable<DomainAndUri> domainUris = _routingHelper.UmbracoDomainsForNode(
				contentId: node.Id,
				current: current,
				excludeDefault: false);

			List<IPublishedContent> parents = new List<IPublishedContent>();

			while (domainUris == null && node != null) // n is null at root
			{
				// move to parent node
				node = node.Parent;

				if (node != null) parents.Add(node);

				domainUris = node == null ? null : _routingHelper.UmbracoDomainsForNode(
					contentId: node.Id,
					current: current,
					excludeDefault: false);
			}

			/*parents.Reverse(); // Reverse from root down to the node*/

			List<string> otherUrls = new List<string>();

			if (domainUris == null)
			{
				// If there are no domains (hostnames) we don't have to do anything
				// because we do not have smth. like /de /en, right?!
				return otherUrls;
			}

			// We have to assemble the other urls by the url alias
			// but we have to do that with all possible languages
			// because the urlAlias could be null but the parent could have one
			/*string path;
			bool isLocalized;
			UrlAlias alias;*/
			string path;
			foreach (CultureInfo cultureInfo in LocalizationContext.Cultures)
			{
				//TODO: We use the same as the Frontend would use it... the question is, if that is ok?
				// Due to performance... I don't know we have less code here but populate always the parents and it's domains?
				path = content.GetLocalizedUrl(language: cultureInfo.TwoLetterISOLanguageName);

				// Url per language will be added because of the domainAndUris
				// so /about-us is possible for all languages e.g. /en/about-us /de/about-us /it/about-us...

				foreach (DomainAndUri domainUri in domainUris)
				{
					// We have to replace the host and the language so we can populate the other urls correctly
					path = path.Replace(domainUri.Uri.GetBaseUrl(), string.Empty);
					path = path.EnsureStartsWith('/').EnsureEndsWith('/');
					path = path.Replace("/" + cultureInfo.TwoLetterISOLanguageName.ToLowerInvariant() + "/", string.Empty);
					path = path.EnsureStartsWith('/').EnsureEndsWith('/');
				}

				//alias = urlAlias.SingleOrDefault(o => o.ISOCode.Equals(cultureInfo.Name));

				//otherUrls.Add(path);

				/*path = "/" + string.Join("/", _routingHelper.GetUrlNamesByISOCode(contents: parents, isoCode: cultureInfo.Name));
				path += (parents.Count > 1 ? "/" : string.Empty) + _routingHelper.GetUrlName(false, content, alias, out isLocalized);*/

				otherUrls.AddRange(AssembleOtherUrls(
					domainUris: domainUris,
					path: path));
			}

			return otherUrls
				   .Where(o => o.Equals(content.Url) == false) // Except the url made from umbraco
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
				if (ContentFinderResolver.Current.ContainsType<Opten.Umbraco.Localization.Web.Routing.ContentFinderByUrlAlias>())
					return true;

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