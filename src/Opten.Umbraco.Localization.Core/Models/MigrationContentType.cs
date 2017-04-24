using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opten.Umbraco.Localization.Core.Models
{
	public class MigrationContentType
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("properties")]
		public IEnumerable<MigrationProperty> Properties { get; set; }
	}
}
