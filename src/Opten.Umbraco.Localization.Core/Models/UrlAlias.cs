using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Core.Models
{
	public class UrlAlias
	{

		[JsonProperty("isoCode")]
		public string ISOCode { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

	}
}