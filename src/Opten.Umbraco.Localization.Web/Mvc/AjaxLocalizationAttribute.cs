using Opten.Common.Extensions;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Mvc
{
	/// <summary>
	/// Localizes the Controller by accept-language's culture.
	/// </summary>
	public class AjaxLocalizationAttribute : ActionFilterAttribute
	{

		//HINT: It's also possible to override Initialize() method from the Controller
		// we do that in some projects like the Merchello Integration
		// maybe sometimes Umbraco provides a better way and we can do all here!

		/// <summary>
		/// Called by the ASP.NET MVC framework before the action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.RequestContext.HttpContext.Request.TrySetCultureFromAcceptLanguage(
					cultures: LocalizationContext.Cultures);
			}

			base.OnActionExecuting(filterContext);
		}

	}
}