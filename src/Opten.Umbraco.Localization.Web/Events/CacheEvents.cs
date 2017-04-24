using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Opten.Umbraco.Localization.Web.Events
{
	internal class CacheEvents : ApplicationEventHandler
	{

		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			DomainService.Saved += DomainService_Saved;
			DomainService.Deleted += DomainService_Deleted;
			LocalizationService.SavedLanguage += LocalizationService_SavedLanguage;
			LocalizationService.DeletedLanguage += LocalizationService_DeletedLanguage;
		}

		void DomainService_Saved(IDomainService sender, SaveEventArgs<IDomain> e)
		{
			ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Core.Constants.Cache.Domains);
		}

		void DomainService_Deleted(IDomainService sender, DeleteEventArgs<IDomain> e)
		{
			ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Core.Constants.Cache.Domains);
		}

		void LocalizationService_SavedLanguage(ILocalizationService sender, SaveEventArgs<ILanguage> e)
		{
			ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Core.Constants.Cache.Languages);
		}

		void LocalizationService_DeletedLanguage(ILocalizationService sender, DeleteEventArgs<ILanguage> e)
		{
			ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(Core.Constants.Cache.Languages);
		}

	}
}