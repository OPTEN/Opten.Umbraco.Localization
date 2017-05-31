using System;
using System.Configuration;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web
{
	internal class Startup : ApplicationEventHandler
	{

		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			bool enableBrowserRedirect;
			bool hasKey = Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:browserLanguageRedirect"], out enableBrowserRedirect);

			if (hasKey && enableBrowserRedirect)
			{
				// By registering this here we can make sure that if route hijacking doesn't find a controller it will use this controller.
				// That way each page will always be routed through one of our controllers.
				DefaultRenderMvcControllerResolver.Current.SetDefaultControllerType(controllerType:
					typeof(Web.Mvc.LocalizedRenderMvcController));

				LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Mvc.LocalizedRenderMvcController");
			}

			bool enable404Provider;
			hasKey = Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:404"], out enable404Provider);

			if (hasKey && enable404Provider)
			{
				//TODO: Performance?
				// for newer Umbraco versions --> InsertBefore<ContentFinderByRedirectUrl>() but we have to support legacy
				// so it's better to insert after ContentFinderByIdPath by getting the index of it
				int index = ContentFinderResolver.Current.GetTypes().IndexOf(typeof(global::Umbraco.Web.Routing.ContentFinderByIdPath));

				// + 1 --> has to be after ContentFinderByIdPath
				index++;

				//TODO: We have to add them because they're not added yet because of legacy
				// https://github.com/umbraco/Umbraco-CMS/blob/master/src/Umbraco.Web/WebBootManager.cs
				// what happens if umbraco adds them?!
				// => Maybe we have to rearrange this because of the ContentLastChanceFinderResolver but this do not work atm
				//    because then the umbracoSettings.config <erro404></error404> comes first and we do not want to have a web.config setting...
				ContentFinderResolver.Current.InsertType<global::Umbraco.Web.Routing.ContentFinderByUrlAlias>(index++); //TODO: Umbraco has it after ContentFinderByProfile but in 404handlers.config it's the first?
				ContentFinderResolver.Current.InsertType<global::Umbraco.Web.Routing.ContentFinderByNiceUrlAndTemplate>(index++);
				ContentFinderResolver.Current.InsertType<global::Umbraco.Web.Routing.ContentFinderByProfile>(index++);
				// VERY IMPORTANT: Has to be the second last content finder! <-----------------
				//TODO: Should this be disabled through the web.config?
				ContentFinderResolver.Current.InsertTypeBefore<global::Umbraco.Web.Routing.ContentFinderByNotFoundHandlers, Web.Routing.ContentFinderByNotFound>();

				LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Routing.ContentFinderByNotFound");
				/*if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["OPTEN:localization:errorPage"]) == false)
				{
					// With the last change finder we can handle 404 (localized)
					ContentLastChanceFinderResolver.Current.SetFinder(new Web.Routing.LocalizedLastChangeFinder());

					LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Routing.LocalizedLastChangeFinder");
				}*/
			}

			bool enableSegmentProvider;
			hasKey = Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:aliasUrlProvider"], out enableSegmentProvider);

			//TODO: Maybe we have to think about the positioning of Web.Routing.ContentFinderByUrlAlias
			// Should it be before Umbraco's ContentFinderByUrlAlias or ContentFinderByNiceUrl or after??
			// IMO: It makes absolutely sense to have it before Umbracos or at least right after ContentFinderByNiceUrl
			// because it's "faster" for the customer --> searchs for correct url or localized url and then maybe template url (devs) or id...
			if (hasKey && enableSegmentProvider)
			{
				// With the url providers we can change content urls.
				UrlProviderResolver.Current.InsertTypeBefore<global::Umbraco.Web.Routing.DefaultUrlProvider, Web.Routing.AliasUrlProvider>();
				LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Routing.AliasUrlProvider");

				// We add it here before ContentFinderByIdPath and after ContentFinderByNiceUrl to prevent that "slower" apps which using something like
				// ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNotFoundHandlers, ContentFinder404>(); 
				// generate the ContentFinderResolver.Current like this:
				//
				// - ...
				// - ContentFinder404 (wrong)
				// - ContentFinderByUrlAlias
				// - ContentFinderByNotFoundHandlers
				//
				// but actually it has to be
				//
				// - ...
				// - ContentFinderByUrlAlias
				// - ContentFinder404 (correct)
				// - ContentFinderByNotFoundHandlers
				ContentFinderResolver.Current.InsertTypeBefore<global::Umbraco.Web.Routing.ContentFinderByIdPath, Web.Routing.ContentFinderByUrlAlias>();
				LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Routing.ContentFinderByUrlAlias");

				ContentFinderResolver.Current.InsertTypeBefore<global::Umbraco.Web.Routing.ContentFinderByIdPath, Web.Routing.ContentFinderByUrlAliasAndTemplate>();
				LogHelper.Info<Startup>("Applied Opten.Umbraco.Localization.Web.Routing.ContentFinderByUrlAliasAndTemplate");
			}
		}

	}
}