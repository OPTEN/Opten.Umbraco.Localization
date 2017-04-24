using Opten.Umbraco.Localization.Web.Mvc;

using Umbraco.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.UI.Controllers
{

	[PluginController("v1")]
	public class TestApiController : LocalizedUmbracoApiController
	{

		public string GetDictionary()
		{
			return Umbraco.GetDictionaryValue("Test");
		}

		public string GetContentUrl(int pageId)
		{
			return Umbraco.TypedContent(pageId).Url;
		}

	}

}