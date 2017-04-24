using Opten.Umbraco.Localization.Web.Mvc;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.UI.Controllers
{

	[PluginController("v1")]
	public class TestController : LocalizedSurfaceController
	{

		public ActionResult HttpUnauthorized()
		{
			//TODO: Here is the language wrong is there a way to fix it?
			return new HttpUnauthorizedResult();
		}

		public ActionResult HttpException()
		{
			// This only works when "existingResponse="Replace" in the <httpErrors />
			throw new HttpException(404, "Couldn't open file!");
		}

		public ActionResult HttpStatusCodeResult()
		{
			// This only works when "existingResponse="Replace" in the <httpErrors />
			return new HttpStatusCodeResult(404);
		}
		
		public ContentResult Dictionary()
		{
			ContentResult result = new ContentResult();

			result.Content = Umbraco.GetDictionaryValue("Test");

			return result;
		}

		public ContentResult ContentUrl(int pageId = 0)
		{
			if (pageId < 1 && UmbracoContext.PageId.HasValue)
			{
				pageId = UmbracoContext.PageId.Value;
			}

			ContentResult result = new ContentResult();

			result.Content = Umbraco.TypedContent(pageId).Url;

			return result;
		}

	}
}