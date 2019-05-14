using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	public class OnUrlGeneratingEventArgs: EventArgs
	{
		public OnUrlGeneratingEventArgs(
			UmbracoContext umbracoContext, 
			int id,
			Uri current,
			UrlProviderMode mode = UrlProviderMode.Auto,
			CultureInfo culture = null)
		{
			UmbracoContext = umbracoContext;
			Id = id;
			CurrentUri = current;
			UrlProviderMode = mode;
			Culture = culture;
			Cancel = false;
		}

		public UmbracoContext UmbracoContext { get; set; }

		public bool Cancel { get; set; }
		public int Id { get; set; }
		public Uri CurrentUri { get; set; }
		public UrlProviderMode UrlProviderMode { get; set; }
		public CultureInfo Culture { get; set; }
	}
}
