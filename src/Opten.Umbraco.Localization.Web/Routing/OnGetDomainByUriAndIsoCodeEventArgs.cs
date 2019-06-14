using System;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	public class OnGetDomainByUriAndIsoCodeEventArgs : EventArgs
	{
		public OnGetDomainByUriAndIsoCodeEventArgs(
			DomainAndUri[] domainsAndUris,
			Uri current,
			string isoCode)
		{
			this.DomainsAndUris = domainsAndUris;
			this.Current = current;
			this.IsoCode = isoCode;
		}
		public DomainAndUri SelectedDomainAndUri { get; set; }

		public DomainAndUri[] DomainsAndUris { get; set; }
		public Uri Current { get; set; }
		public string IsoCode { get; set; }
	}
}
