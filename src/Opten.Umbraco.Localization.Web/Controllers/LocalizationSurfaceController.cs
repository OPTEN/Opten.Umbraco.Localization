using Opten.Umbraco.Localization.Web.Application;
using Opten.Umbraco.Localization.Web.Session;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Models = Opten.Umbraco.Localization.Core.Models;

namespace Opten.Umbraco.Localization.Web.Controllers
{
	[PluginController("Localization")]
	public class LocalizationSurfaceController : SurfaceController
	{

		public IEnumerable<Models.Language> GetSelectedLanguages()
		{
			return LocalizationSession.Current.SelectedLanguages;
		}

		
	}
}
