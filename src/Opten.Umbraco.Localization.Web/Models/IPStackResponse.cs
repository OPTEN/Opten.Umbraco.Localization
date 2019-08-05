using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Web.Models
{
	public class IPStackResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("error")]
		public IPStackError Error { get; set; }

		[JsonProperty("ip")]
		public string IP { get; set; }

		[JsonProperty("continent_name")]
		public string ContinentName { get; set; }

		[JsonProperty("country_code")]
		public string CountryCode { get; set; }

		[JsonProperty("country_name")]
		public string CountryName { get; set; }
	}
	public class IPStackError
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("info")]
		public string Info { get; set; }
	}
}
