using Newtonsoft.Json;

namespace Opten.Umbraco.Localization.Core.Models
{
	public class MigrationProperty
	{

		[JsonProperty("group")]
		public string Group { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("propertyEditorAlias")]
		public string PropertyEditorAlias { get; set; }

		[JsonProperty("dataTypeDefinitionId")]
		public int DataTypeDefinitionId { get; set; }

		[JsonProperty("localize")]
		public bool Localize { get; set; }

		[JsonProperty("contentTypeAlias")]
		public string ContentTypeAlias { get; set; }
	}
}