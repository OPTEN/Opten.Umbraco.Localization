using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Web.Models
{
	public class IPStackResponse
	{
		[JsonProperty("ip")]
		public string IP { get; set; }

		[JsonProperty("continent_name")]
		public string ContinentName { get; set; }

		[JsonProperty("country_code")]
		public string CountryCode { get; set; }

		[JsonProperty("country_name")]
		public string CountryName { get; set; }
	}
}
