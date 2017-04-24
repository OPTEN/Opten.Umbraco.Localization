using Opten.Umbraco.Localization.Web.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Opten.Umbraco.Localization.Web.Routing
{
	internal class LocalizedRouting
	{

		private readonly ApplicationContext _applicationContext;
		private readonly UmbracoContext _umbracoContext;

		private readonly IDomainService _domainService;

		public LocalizedRouting(
			ApplicationContext applicationContext,
			UmbracoContext umbracoContext)
		{
			Mandate.ParameterNotNull(applicationContext, "applicationContext");
			Mandate.ParameterNotNull(umbracoContext, "umbracoContext");

			_applicationContext = applicationContext;
			_umbracoContext = umbracoContext;
			_domainService = _applicationContext.Services.DomainService;
		}

		internal CultureInfo TryGetCultureByRequest(HttpRequestBase request)
		{
			// Get the browser language (and check if valid)
			// and check it against umbraco languages
			// because maybe there is a language de-CH but a hostname de-CH is never applied!
			CultureInfo culture = request.TryGetCultureFromBrowser();

			//TODO: Get url from request.Url when PageId not has value?

			bool? hasDomain = _umbracoContext?.PublishedContentRequest?.HasDomain;

			if (hasDomain.HasValue && hasDomain.Value)
			{
				// We have to check if the domain exists (if a hostname is assigned)
				// otherwise we force an error 404!
				return _umbracoContext.PublishedContentRequest.UmbracoDomain.TryGetCulture(culture);
			}

			IDomain[] domains = GetDomains();

			// We have to check if the domain exists (if a hostname is assigned)
			// otherwise we force an error 404!
			return domains.TryGetCulture(culture: culture);
		}

		private IDomain[] GetDomains()
		{
			// Cache the Domains so we don't have to call the database for redirects
			return _applicationContext.ApplicationCache.RuntimeCache.GetCacheItem(
				cacheKey: Core.Constants.Cache.Domains,
				getCacheItem: () =>
				{
					return _domainService.GetAll(
						includeWildcards: false).ToArray();
				},
				timeout: TimeSpan.FromHours(24)) as IDomain[];
		}

	}
}