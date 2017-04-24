using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Core.Models
{
	public class Language
	{

		[JsonProperty("isoCode")]
		public string ISOCode { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("nativeName")]
		public string NativeName { get; set; }

		[JsonProperty("isDefault")]
		public bool IsDefault { get; set; }

	}
}