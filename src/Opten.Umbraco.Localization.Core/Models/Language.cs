using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Core.Models
{
	public class Language
	{

		[JsonProperty("isoCode")]
		public string ISOCode { get; set; }

		[JsonProperty("languageAlias")]
		public string LanguageAlias { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("isDefault")]
		public bool IsDefault { get; set; }

	}
}