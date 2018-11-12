using Opten.Common.Extensions;
using Opten.Common.Helpers;
using Opten.Umbraco.Localization.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	internal class RoutingHelper
	{

		private readonly IDomainService _domainService;

		public RoutingHelper(IDomainService domainService)
		{
			_domainService = domainService;
		}

		//TODO: Performance, Caching?
		//TODO: Has to be changed for 7.3.x

		/// <summary>
		/// Gets the localized route and checks if any is set.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="preview">if set to <c>true</c> [preview].</param>
		/// <param name="contentId">The content identifier.</param>
		/// <param name="isoCode">The ISO code.</param>
		/// <param name="anyLocalizedUrl">if set to <c>true</c> [any localized URL].</param>
		/// <returns></returns>
		/// https://github.com/umbraco/Umbraco-CMS/blob/ded1def8e2e7ea1a4fd0f849cc7a3f1f97cd8242/src/Umbraco.Web/PublishedCache/XmlPublishedCache/PublishedContentCache.cs
		internal string GetRouteById(
			UmbracoContext umbracoContext,
			bool preview,
			int contentId,
			string isoCode,
			out bool anyLocalizedUrl)
		{
			// This means an editor set a localized url in the backend
			anyLocalizedUrl = false;

			IPublishedContent content = umbracoContext.ContentCache.GetById(
				preview: preview,
				contentId: contentId);

			if (content == null) return string.Empty;

			// walk up from that node until we hit a node with a domain, 
			// or we reach the content root, collecting urls in the way 
			List<string> pathParts = new List<string>();
			IPublishedContent node = content;
			bool hasDomains = this.NodeHasDomains(contentId: node.Id);
			bool isLocalized;
			while (hasDomains == false && node != null) // n is null at root 
			{
				pathParts.Add(GetUrlNameByISOCode(node, isoCode, out isLocalized));
				if (isLocalized) anyLocalizedUrl = true; // only set this if we have one

				// move to parent node 
				node = node.Parent;
				hasDomains = node != null && this.NodeHasDomains(contentId: node.Id);
			}

			// assemble the route 
			pathParts = pathParts.Where(o => o.Equals("/") == false) //TODO: Can we do that better?
								 .ToList();
			pathParts.Reverse();

			string path = "/" + string.Join("/", pathParts); // will be "/" or "/foo" or "/foo/bar" etc 
			string route = (node == null ? string.Empty : node.Id.ToString(CultureInfo.InvariantCulture)) + path;

			return route;
		}

		/// <summary>
		/// Assembles the URL with the found hostname.
		/// </summary>
		/// <param name="route">The route.</param>
		/// <param name="current">The current.</param>
		/// <param name="mode">The mode.</param>
		/// <param name="isoCode">The iso code.</param>
		/// <returns></returns>
		internal string AssembleUrl(
			string route,
			Uri current,
			UrlProviderMode mode,
			string isoCode)
		{
			// extract domainUri and path 
			// route is /<path> or <domainRootId>/<path> 
			int pos = route.IndexOf('/');
			string path = pos == 0 ? route : route.Substring(pos);

			DomainAndUri domainUri = pos == 0 ? null : this.DomainForNode(
				contentId: int.Parse(route.Substring(0, pos)),
				current: current,
				isoCode: isoCode);

			return UmbracoAssembleUrl(
				domainUri: domainUri,
				path: path,
				current: current,
				mode: mode).ToString();
		}

		internal bool HasUrlName(IPublishedContent content, string urlName)
		{
			if (content == null) return false;

			if (content.UrlName.Equals(urlName, StringComparison.InvariantCultureIgnoreCase)) return true;

			UrlAlias[] urlAlias = GetUrlAlias(content: content);

			if (urlAlias == null || urlAlias.Any() == false) return false;

			return urlAlias.Any(o => o.Url.Equals(urlName, StringComparison.InvariantCultureIgnoreCase));
		}

		internal string GetUrlNameByISOCode(IPublishedContent content, string isoCode, out bool isLocalized)
		{
			UrlAlias urlAlias = GetUrlAlias(content: content, isoCode: isoCode);

			return GetUrlName(content, urlAlias, out isLocalized);
		}

		internal string GetUrlName(IPublishedContent content, UrlAlias urlAlias, out bool isLocalized)
		{
			isLocalized = false;

			if (content == null) return string.Empty;

			if (urlAlias == null)
			{
				return content.UrlName;
			}
			else
			{
				isLocalized = (IsEmptyUrlName(urlAlias: urlAlias) == false);

				return isLocalized
						? urlAlias.Url
						: content.UrlName;
			}
		}

		internal UrlAlias[] GetUrlAlias(IPublishedContent content)
		{
			UrlAlias[] urlAlias = new UrlAlias[0];


			if (content != null &&
				content.HasProperty(Core.Constants.Conventions.Content.UrlAlias) &&
				content.HasValue(Core.Constants.Conventions.Content.UrlAlias))
			{
				urlAlias = content.GetPropertyValue<UrlAlias[]>(Core.Constants.Conventions.Content.UrlAlias);
			}

			return urlAlias ?? new UrlAlias[0];
		}

		private UrlAlias GetUrlAlias(IPublishedContent content, string isoCode)
		{
			UrlAlias[] urlAlias = GetUrlAlias(content: content);

			if (urlAlias == null || urlAlias.Any() == false) return null;

			return urlAlias.FirstOrDefault(o => o.ISOCode.Equals(isoCode, StringComparison.OrdinalIgnoreCase));
		}

		private bool IsEmptyUrlName(UrlAlias urlAlias)
		{
			return string.IsNullOrWhiteSpace(urlAlias.Url) || string.IsNullOrWhiteSpace(urlAlias.Url.ToUrlSegment());
		}

		#region Copied from DomainHelper.cs because it's internal ;-(

		private Uri UmbracoAssembleUrl(DomainAndUri domainUri, string path, Uri current, UrlProviderMode mode)
		{
			// This method is private in umbraco, but we want to use their!
			DefaultUrlProvider provider = ActivatorHelper.CreateInstance<DefaultUrlProvider>();

			return ActivatorHelper.GetPrivateMethodReturnValueOfInstance<Uri>(
				instance: provider,
				methodName: "AssembleUrl",
				methodArguments: new object[] { domainUri, path, current, mode });
		}

		internal IEnumerable<DomainAndUri> UmbracoDomainsForNode(int contentId, Uri current, bool excludeDefault = true)
		{
			// In 7.3.x the method "DomainsForNode" is not static anymore!
			DomainHelper domainHelper = new DomainHelper(
				domainService: _domainService);

			// Should be removed until DomainHelper is public
			return ActivatorHelper.GetPrivateMethodReturnValueOfInstance<IEnumerable<DomainAndUri>>(
				instance: domainHelper,
				methodName: "DomainsForNode",
				methodArguments: new object[] { contentId, current, excludeDefault });
		}

		internal bool NodeHasDomains(int contentId)
		{
			if (contentId < 1) return false;

			IEnumerable<IDomain> domains = _domainService.GetAssignedDomains(
				contentId: contentId,
				includeWildcards: false);

			return domains != null && domains.Any();
		}

		#endregion

		#region Private methods

		private DomainAndUri DomainForNode(int contentId, Uri current, string isoCode)
		{
			// sanitize the list to have proper uris for comparison (scheme, path end with /)
			// we need to end with / because example.com/foo cannot match example.com/foobar
			// we need to order so example.com/foo matches before example.com/
			string scheme = current == null ? Uri.UriSchemeHttp : current.Scheme;

			IEnumerable<IDomain> domains = _domainService.GetAssignedDomains(
				contentId: contentId,
				includeWildcards: false);

			// No domains so return null
			if (domains.Any() == false) return null;

			DomainAndUri[] domainsAndUris = domains
				.Where(d => d.IsWildcard == false)
				//.Select(SanitizeForBackwardCompatibility) // Not needed because we have pre-4.10+
				.Select(d => new DomainAndUri(d, scheme))
				.OrderByDescending(d => d.Uri.ToString())
				.ToArray();

			// this is easier than umbraco's approach
			// because for this localization it's only allowed to have one culture per hostname
			// maybe we have to change this for the future...

			// look for the first domain that would be the base of the current url and has same the iso code
			// ie current is www.example.com/foo/bar, look for domain www.example.com 
			DomainAndUri domainAndUri = TryGetDomainByUriAndIsoCode(
				domainsAndUris: domainsAndUris,
				current: current,
				isoCode: isoCode);

			// if nothing found we get at least the correct language
			if (domainAndUri == null)
			{
				domainAndUri = domainsAndUris.FirstOrDefault(
					o => o.UmbracoDomain.LanguageIsoCode == isoCode);
			}

			return domainAndUri;
		}

		private DomainAndUri TryGetDomainByUriAndIsoCode(
			DomainAndUri[] domainsAndUris,
			Uri current,
			string isoCode)
		{
			current = current.EndPathWithSlash();

			// Is it okay that we only check the BaseUrl()?
			// I think so, because we do not care about the rest...
			return domainsAndUris.FirstOrDefault(o =>
				o.Uri.GetBaseUrl().Equals(current.GetBaseUrl())
					//o.Uri.EndPathWithSlash().IsBaseOf(current)
					//||
					//o.Uri.EndPathWithSlash().IsBaseOf(current.WithoutPort())
					//)
				&&
				o.UmbracoDomain.LanguageIsoCode.Equals(isoCode));
		}

		#endregion

	}
}