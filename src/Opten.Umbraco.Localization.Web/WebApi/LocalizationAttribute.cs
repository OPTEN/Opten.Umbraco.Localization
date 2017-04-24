using Opten.Common.Extensions;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Opten.Umbraco.Localization.Web.WebApi
{

#pragma warning disable 1591

	/// <summary>
	/// Localizes the Controller by accept-language's culture.
	/// </summary>
	public class LocalizationAttribute : ActionFilterAttribute
	{

		//HINT: It's also possible to override Initialize() method from the Controller
		// we do that in some projects like the Merchello Integration
		// maybe sometimes Umbraco provides a better way and we can do all here!

		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			actionContext.Request.TrySetCultureFromAcceptLanguage(
				cultures: LocalizationContext.Cultures);

			base.OnActionExecuting(actionContext);
		}

	}

#pragma warning restore 1591

}