using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Extensions;
using Opten.Umbraco.Localization.Web.Routing;
using System;
using System.Globalization;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Mvc
{

#pragma warning disable 1591

	/// <summary>
	/// The Localization Render MVC Controller (mostly used to override the default RenderMvcController).
	/// </summary>
	[PreRenderViewActionFilter]
	public class LocalizedRenderMvcController : RenderMvcController
	{
		public static event EventHandler<LocalizedBeforeActionExecutingEventArgs> BeforeActionExecuting;

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			bool cancel = false;
			if (BeforeActionExecuting != null)
			{
				LocalizedBeforeActionExecutingEventArgs e = new LocalizedBeforeActionExecutingEventArgs(filterContext);
				BeforeActionExecuting(this, e);
				cancel = e.Cancel;
			}

			if (cancel)
			{
				base.OnActionExecuting(filterContext);
				return;
			}

			try
			{
				// Pass through preview mode from umbraco
				if (UmbracoContext.InPreviewMode || UmbracoContext.IsFrontEndUmbracoRequest == false)
				{
					base.OnActionExecuting(filterContext);
					return;
				}

				// Pass through errors
				if (Response.StatusCode == 404)
				{
					//TODO: Is there a nicer way for handling errors?
					//      the problem is, that 1000.aspx is not found > redirect > again not found > redirect ... (infinite loop)
					// But I think it's okay because when 404 occures, there should be a language or the URL do not exists anyways.
					// Why we then have to redirect to the correct language?
					// OR you could create error 404 with all translations in it
					// OR multiple error pages in umbracoSettings.config (with cultures) => NOT TESTED
					// OR try to find the error page and redirect with language if found
					base.OnActionExecuting(filterContext);
					return;
				}

				// When url is an umbraco url (e.g. umbraco forms makes problem with /Umbraco/RenderMvc) do nothing
				if (Request.Url.AbsolutePath.StartsWith("/umbraco/", StringComparison.OrdinalIgnoreCase))
				{
					//TODO: Is there a nicer way to know that?
					base.OnActionExecuting(filterContext);
					return;
				}

				// When url contains language do nothing
				if (Request.Url.ContainsLanguage())
				{
					base.OnActionExecuting(filterContext);
					return;
				}

				// When is the same as the template do nothing
				if (UmbracoContext.PublishedContentRequest.HasTemplate)
				{
					//TODO: What if page Newsletter and template Newsletter?
					//TODO: Is there a nicer way to know that? E.g. by ContentFinderByNiceUrlAndTemplate
					string template = Request.Url.GetUrl(withQuery: false, withDomain: false);

					if (template.RemoveLeadingAndTrailingSlashAndBackslash().Equals(
						UmbracoContext.PublishedContentRequest.TemplateAlias, StringComparison.OrdinalIgnoreCase))
					{
						base.OnActionExecuting(filterContext);
						return;
					}
				}

				// Get culture from browser or application
				LocalizedRouting router = new LocalizedRouting(
					applicationContext: ApplicationContext,
					umbracoContext: UmbracoContext);

				CultureInfo culture = router.TryGetCultureByRequest(Request);

				// otherwise redirect to language
				string redirectUrl = Request.Url.GetUrlWithLanguage(
					language: culture.GetUrlLanguage(),
					withQuery: true,
					withDomain: false);

				//TODO: UrlAliasProvider to get the "correct" url?
				/*if (Routing.AliasUrlProvider.FindByUrlAliasEnabled &&
					UmbracoContext.PublishedContentRequest != null &&
					UmbracoContext.PublishedContentRequest.HasPublishedContent)
				{
					redirectUrl = UmbracoContext.PublishedContentRequest.PublishedContent.GetLocalizedUrl(language: culture.GetUrlLanguage());
				}*/

				filterContext.Result = new RedirectResult(url: redirectUrl);

				base.OnActionExecuting(filterContext);
			}
			catch (Exception ex)
			{
				LogHelper.Error<LocalizedRenderMvcController>("Couldn't redirect to language!", ex);

				// If we couldn't redirect we don't want to force an error 500
				// better exectue the base method.
				base.OnActionExecuting(filterContext);
			}
		}

	}

#pragma warning restore 1591

}
