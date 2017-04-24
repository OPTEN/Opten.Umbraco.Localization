using Opten.Umbraco.Localization.Core.Models;
using Opten.Umbraco.Localization.Web.Helpers;
using System.Collections.Generic;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Opten.Umbraco.Localization.Web.Controllers
{

#pragma warning disable 1591

	[PluginController("OPTEN"), IsBackOffice]
	public sealed class MigrationApiController : UmbracoAuthorizedJsonController
	{

		private readonly ContentTypeMigration _migrater;

		public MigrationApiController()
		{
			_migrater = new ContentTypeMigration(
				contentTypeService: Services.ContentTypeService,
				dataTypeService: Services.DataTypeService);
		}

		public IEnumerable<MigrationContentType> GetAllContentTypes()
		{
			return _migrater.ConvertToMigrationContentType(Services.ContentTypeService.GetAllContentTypes());
		}

		public void PostLocalize(IEnumerable<MigrationContentType> contentTypes)
		{
			foreach (MigrationContentType contentType in contentTypes)
			{
				foreach (MigrationProperty property in contentType.Properties)
				{
					if (property.Localize)
					{
						_migrater.UpdateContentType(contentType.Alias, property.Alias);
					}
				}
			}
		}

		public void PostLocalizeAll()
		{
			foreach (MigrationContentType contentType in GetAllContentTypes())
			{
				foreach (MigrationProperty property in contentType.Properties)
				{
					if (PropertyHelper.IsLocalizedProperty(property.Alias))
					{
						_migrater.UpdateContentType(contentType.Alias, property.Alias);
					}
				}
			}
		}
	}

#pragma warning restore 1591

}
