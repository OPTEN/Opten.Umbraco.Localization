using System.Globalization;
using System.Threading;
using System.Web;

using umbraco.interfaces;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	public class Handle404 : INotFoundHandler
	{

		public bool CacheUrl { get { return false; } }

		public int redirectID { get; set; }

		public bool Execute(string url)
		{
			this.redirectID = 0;

			// Take the umbraco one to assemble the redirect id.
			// (because some things are internal :-(...)
			umbraco.handle404 handler = new umbraco.handle404();

			if (handler.Execute(url))
			{
				this.redirectID = handler.redirectID;

				PublishedContentRequest contentRequest = UmbracoContext.Current.PublishedContentRequest;

				if (contentRequest == null) return false;

				LocalizedRouting router = new LocalizedRouting(
					applicationContext: contentRequest.RoutingContext.UmbracoContext.Application,
					umbracoContext: contentRequest.RoutingContext.UmbracoContext);

				HttpContext context = HttpContext.Current;

				if (context == null) return false;

				HttpRequestWrapper request = new HttpRequestWrapper(
					httpRequest: context.Request);

				CultureInfo culture = router.TryGetCultureByRequest(
					request: request);

				// Set correct culture
				Thread.CurrentThread.CurrentUICulture = culture;
				Thread.CurrentThread.CurrentCulture = culture;
				contentRequest.Culture = culture;

				// Do the redirect
				return true;
			}

			return false;
		}
	}
}