using Opten.Common.Extensions;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Mvc
{

#pragma warning disable 1591

	/// <summary>
	/// The Localization Umbraco Surface Controller.
	/// </summary>
	public abstract class LocalizedSurfaceController : SurfaceController
	{

		protected override void Initialize(RequestContext requestContext)
		{
			if (requestContext.HttpContext.Request.IsAjaxRequest())
			{
				requestContext.HttpContext.Request.TrySetCultureFromAcceptLanguage(
					cultures: LocalizationContext.Cultures);
			}

			base.Initialize(requestContext);
		}

	}

#pragma warning restore 1591

}