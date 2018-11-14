using System;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{

#pragma warning disable 1591

	public class LocalizedUri
	{

		public string Route { get; internal set; }

		public string Url { get; internal set; }

		public bool Localized { get; internal set; }

	}

#pragma warning restore 1591

}