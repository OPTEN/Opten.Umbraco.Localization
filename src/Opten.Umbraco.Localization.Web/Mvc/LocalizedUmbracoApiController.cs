using Opten.Common.Extensions;
using System.Web.Http.Controllers;
using Umbraco.Web.WebApi;

namespace Opten.Umbraco.Localization.Web.Mvc
{

#pragma warning disable 1591

	/// <summary>
	/// The Localization Umbraco API Controller.
	/// </summary>
	public abstract class LocalizedUmbracoApiController : UmbracoApiController
	{

		protected override void Initialize(HttpControllerContext controllerContext)
		{
			controllerContext.Request.TrySetCultureFromAcceptLanguage(
				cultures: LocalizationContext.Cultures);

			base.Initialize(controllerContext);
		}

	}

#pragma warning restore 1591

}