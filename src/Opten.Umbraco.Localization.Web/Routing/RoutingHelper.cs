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
		/// Gets the localized URI.
		/// </summary>
		/// <param name="umbracoContext">The umbraco context.</param>
		/// <param name="contentId">The content identifier.</param>
		/// <param name="isoCode">The ISO code.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">content</exception>
		/// https://github.com/umbraco/Umbraco-CMS/blob/ded1def8e2e7ea1a4fd0f849cc7a3f1f97cd8242/src/Umbraco.Web/PublishedCache/XmlPublishedCache/PublishedContentCache.cs
		internal LocalizedUri GetLocalizedUri(
			UmbracoContext umbracoContext,
			int contentId,
			string isoCode)
		{
			// will not use cache if previewing
			IPublishedContent content = umbracoContext.ContentCache.GetById(contentId);

			if (content == null)
			{
				return null;
			}

			LocalizedUri routing = new LocalizedUri();

			string route = umbracoContext.ContentCache.GetRouteById(contentId);

			// extract domainRootId and path
			// route is /<path> or <domainRootId>/<path>
			int pos = route.IndexOf('/');
			int domainRootId = pos == 0 ? 0 : int.Parse(route.Substring(0, pos));

			// walk up from that node until we hit a node with a domain, 
			// or we reach the content root, collecting urls in the way 
			List<string> pathParts = new List<string>();
			IPublishedContent node = content;
			while (node != null && node.Id != domainRootId) // node is null at root 
			{
				pathParts.Add(GetUrlNameByISOCode(node, isoCode, out bool localized));

				if (localized)
				{
					routing.Localized = true;
				}

				// move to parent node 
				node = node.Parent;
			}

			pathParts.Reverse();

			routing.Route = domainRootId + string.Join("/", pathParts).EnsureStartsWith('/');

			return routing;
		}

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

		internal string GetUrlNameByISOCode(IPublishedContent content, string isoCode, out bool localized)
		{
			UrlAlias urlAlias = GetUrlAlias(content: content, isoCode: isoCode);

			return GetUrlName(content, urlAlias, out localized);
		}

		internal string GetUrlName(IPublishedContent content, UrlAlias urlAlias, out bool localized)
		{
			localized = false;

			if (content == null) return string.Empty;

			if (urlAlias == null)
			{
				return content.UrlName;
			}
			else
			{
				localized = IsEmptyUrlName(urlAlias: urlAlias) == false;

				return localized
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

		internal bool HasUrlAlias(IPublishedContent content)
		{
			if (content == null)
			{
				return false;
			}

			UrlAlias[] urlAlias = GetUrlAlias(content);

			return urlAlias != null && urlAlias.Any(alias => IsEmptyUrlName(alias) == false);
		}

		private UrlAlias GetUrlAlias(IPublishedContent content, string isoCode)
		{
			UrlAlias[] urlAlias = GetUrlAlias(content: content);

			if (urlAlias == null || urlAlias.Any() == false) return null;

			return urlAlias.FirstOrDefault(o => o.ISOCode.Equals(isoCode, StringComparison.OrdinalIgnoreCase));
		}

		private bool IsEmptyUrlName(UrlAlias urlAlias)
		{
			if (urlAlias == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace(urlAlias.Url) || string.IsNullOrWhiteSpace(urlAlias.Url.ToUrlSegment());
		}

		#region Private methods

		private DomainAndUri DomainForNode(int contentId, Uri current, string isoCode)
		{
			if (contentId < 1)
			{
				return null;
			}

			// get the domains on that node
			IDomain[] domains = _domainService.GetAssignedDomains(contentId, false).ToArray();

			// none?
			if (domains.Any() == false)
			{
				return null;
			}

			string twoLetterISOCode = new CultureInfo(isoCode).TwoLetterISOLanguageName;

			string scheme = current == null ? Uri.UriSchemeHttp : current.Scheme;

			// get domains for the iso code
			DomainAndUri[] domainsAndUris = domains
				.Where(d => d.IsWildcard == false)
				//.Where(d => d.LanguageIsoCode.StartsWith(twoLetterISOCode, StringComparison.OrdinalIgnoreCase))
				.Select(d => new DomainAndUri(d, scheme))
				.OrderByDescending(d => d.Uri.ToString())
				.ToArray();

			if (domainsAndUris.Any() == false)
			{
				// maybe Umbraco can give us the correct domain
				return UmbracoDomainForNode(contentId, current);
			}

			if (current == null)
			{
				// take the first one by default (what else can we do?)
				return domainsAndUris.First();
			}
			else
			{
				// look for the first domain that would be the exact of the current url
				// ie current is www.example.com/de, look for domain www.example.com
				Uri currentWithSlash = current.EndPathWithSlash();

				DomainAndUri domainAndUri = domainsAndUris.FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash));

				if (domainAndUri != null)
				{
					return domainAndUri;
				}

				// look for the first domain that would be the base of the current url
				// ie current is www.example.com/de, look for domain www.example.com/(de)
				string authority = current.GetLeftPart(UriPartial.Authority);
				string path = twoLetterISOCode.EnsureStartsWith('/');

				currentWithSlash = new Uri(authority + path, UriKind.Absolute).EndPathWithSlash();

				domainAndUri = domainsAndUris.FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash));

				if (domainAndUri != null)
				{
					return domainAndUri;
				}

				// if none matches, try again without the port
				// ie current is www.example.com:1234/foo/bar, look for domain www.example.com
				domainAndUri = domainsAndUris.FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash.WithoutPort()));

				if (domainAndUri != null)
				{
					return domainAndUri;
				}
			}

			// maybe Umbraco can give us the correct domain
			return UmbracoDomainForNode(contentId, current);
		}

		#endregion

		#region Copied from DomainHelper.cs because these methods are internal

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

		private DomainAndUri UmbracoDomainForNode(int contentId, Uri current)
		{
			// In 7.3.x the method "DomainsForNode" is not static anymore!
			DomainHelper domainHelper = new DomainHelper(
				domainService: _domainService);

			// Should be removed until DomainHelper is public
			return ActivatorHelper.GetPrivateMethodReturnValueOfInstance<DomainAndUri>(
				instance: domainHelper,
				methodName: "DomainForNode",
				methodArguments: new object[] { contentId, current });
		}

		#endregion

	}
}