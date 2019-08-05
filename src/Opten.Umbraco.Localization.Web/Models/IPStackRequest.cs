using System;

namespace Opten.Umbraco.Localization.Web.Models
{
	internal class IPStackRequest
	{
		internal string IP { get; set; }
		internal string CountryCode { get; set; }
		internal DateTime Date { get; set; }

		internal IPStackRequest(string ipAddress, string countryCode)
		{
			IP = ipAddress;
			CountryCode = countryCode;
			Date = DateTime.Now;
		}
	}
}
