using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{

#pragma warning disable 1591

	public class ContentFinderByNotFound : IContentFinder
	{

		public bool TryFindContent(PublishedContentRequest contentRequest)
		{
			//if (contentRequest.Is404 == false) return false;

			//TODO: Wait until umbraco get rid of this see: https://github.com/umbraco/Umbraco-CMS/blob/d50e49ad37fd5ca7bad2fd6e8fc994f3408ae70c/src/Umbraco.Web/WebBootManager.cs
			//      the 404handlers.config will be removed!
			//      but we need this atm to assemble the redirect id from the umbracoSettings.config <error404></error404>
			ContentFinderByLegacy404 legacyFinder = new ContentFinderByLegacy404();

			bool found = legacyFinder.TryFindContent(contentRequest) &&
						 contentRequest.HasPublishedContent;

			if (found)
			{
				// Get culture from browser or application
				LocalizedRouting router = new LocalizedRouting(
					applicationContext: ApplicationContext.Current,
					umbracoContext: contentRequest.RoutingContext.UmbracoContext);

				// Set the culture for localization
				contentRequest.Culture = router.TryGetCultureByRequest(
					request: contentRequest.RoutingContext.UmbracoContext.HttpContext.Request);
				
				LogHelper.Debug<ContentFinderByNotFound>("Finder found valid content with id={0}.", () => contentRequest.PublishedContent.Id);
				LogHelper.Debug<ContentFinderByNotFound>("Finder set status code to 404.");
			}

			return found;
		}

	}

#pragma warning restore 1591

}