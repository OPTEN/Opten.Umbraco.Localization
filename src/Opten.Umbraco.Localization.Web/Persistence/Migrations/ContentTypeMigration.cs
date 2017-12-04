using Opten.Umbraco.Localization.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Opten.Umbraco.Localization.Web.Helpers
{

#pragma warning disable 1591

	internal class ContentTypeMigration
	{

		private readonly IContentTypeService _contentTypeService;
		private readonly IDataTypeService _dataTypeService;

		internal ContentTypeMigration(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
		{
			_contentTypeService = contentTypeService;
			_dataTypeService = dataTypeService;
		}

		internal void UpdateContentType(string contentTypeAlias, string propertyAlias)
		{
			IContentType contentType = _contentTypeService.GetContentType(contentTypeAlias);
			PropertyType propertyType = contentType.PropertyTypes.FirstOrDefault(o => o.Alias.Equals(propertyAlias));

			this.UpdateContentType(contentType, propertyType);
		}

		internal void UpdateContentType(IContentType contentType, PropertyType propertyType)
		{
			if (propertyType != null)
			{
				IDataTypeDefinition dataTypeDefinition = _dataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId);
				PropertyGroup propertyGroup = contentType.PropertyGroups.FirstOrDefault(o => o.PropertyTypes.Contains(propertyType.Alias));
				var allFollowingPropertyTypes = propertyGroup.PropertyTypes.Where(o => o != propertyType && o.SortOrder >= propertyType.SortOrder).OrderBy(o => o.SortOrder).ToList();

				var sortOrderIndex = propertyType.SortOrder + 1;
				if (PropertyHelper.IsLocalizedProperty(propertyType.Alias) == false)
				{
					propertyType.Alias = PropertyHelper.GetAlias(propertyType.Alias, LocalizationContext.DefaultCulture.TwoLetterISOLanguageName);
				}

				foreach (ILanguage language in LocalizationContext.Languages)
				{
					string localizedAlias = PropertyHelper.GetAlias(PropertyHelper.GetNotLocalizedAlias(propertyType.Alias), language.CultureInfo.TwoLetterISOLanguageName);

					if ((contentType.PropertyTypeExists(localizedAlias) == false ||
						propertyType.Alias.Equals(localizedAlias) == false) &&
						propertyType.PropertyEditorAlias.Equals("OPTEN.UrlAlias", StringComparison.OrdinalIgnoreCase) == false)
					{
						PropertyType newPropertyType = new PropertyType(dataTypeDefinition)
						{
							Alias = localizedAlias,
							Name = propertyType.Name,
							Description = propertyType.Description,
							SortOrder = sortOrderIndex
						};

						contentType.AddPropertyType(newPropertyType, propertyGroup.Name);
						newPropertyType.SortOrder = sortOrderIndex; // Somehow umbraco sets a sort order on adding... but not always?!?
						sortOrderIndex++;
					}
				}

				foreach(var followingPropertyType in allFollowingPropertyTypes)
				{
					propertyGroup.PropertyTypes.Single(o => followingPropertyType == o).SortOrder = sortOrderIndex;
					sortOrderIndex++;
				}
			}

			_contentTypeService.Save(contentType);
		}

		internal IEnumerable<MigrationContentType> ConvertToMigrationContentType(IEnumerable<IContentType> contentTypes)
		{
			List<MigrationContentType> migrationContentTypes = new List<MigrationContentType>();

			foreach (IContentType contentType in contentTypes.OrderBy(o => o.Level).ThenBy(o => o.SortOrder))
			{
				List<MigrationProperty> migrationProperties = new List<MigrationProperty>();

				foreach (PropertyGroup group in contentType.PropertyGroups.OrderBy(o => o.SortOrder))
				{
					foreach (PropertyType property in group.PropertyTypes.OrderBy(o => o.SortOrder))
					{
						if (property.PropertyEditorAlias.Equals("OPTEN.UrlAlias", StringComparison.OrdinalIgnoreCase) == false)
						{
							migrationProperties.Add(new MigrationProperty
							{
								Group = group.Name,
								Name = property.Name,
								Alias = property.Alias,
								PropertyEditorAlias = property.PropertyEditorAlias,
								DataTypeDefinitionId = property.DataTypeDefinitionId,
								Localize = false
							});
						}
					}
				}

				migrationContentTypes.Add(new MigrationContentType
				{
					Alias = contentType.Alias,
					Name = contentType.Name,
					Properties = migrationProperties
				});
			}

			return migrationContentTypes;
		}
	}

#pragma warning restore 1591

}
