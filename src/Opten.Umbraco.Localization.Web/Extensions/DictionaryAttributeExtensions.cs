using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Attributes;
using System.Reflection;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	internal static class DictionaryAttributeExtensions
	{

		internal static string GetDisplayName(this PropertyInfo propertyInfo)
		{
			string displayName = propertyInfo.Name;

			if (propertyInfo.HasCustomAttribute<DictionaryDisplayNameAttribute>())
			{
				DictionaryDisplayNameAttribute attribute = propertyInfo.GetCustomAttribute<DictionaryDisplayNameAttribute>();
				displayName = attribute.DisplayName;
			}

			return displayName;
		}

	}
}
