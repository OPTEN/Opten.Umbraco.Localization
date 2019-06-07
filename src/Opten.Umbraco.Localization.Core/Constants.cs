namespace Opten.Umbraco.Localization.Core
{
	public static class Constants
	{

		public static class Conventions
		{

			public static class Content
			{

				public const string UrlAlias = "optenUrlAlias"; // Has to be the property alias and not editor alias because we need to find it in the xpath!

			}

		}

		public static class Cache
		{

			public const string Domains = "OPTEN:localization:domains:all";

			public const string Languages = "OPTEN:localization:languages:all";

			public const string Country = "OPTEN:localization:country";

			public const string BackendLanguages = "OPTEN.localization.backend.languages"; // Has to be valid characters otherwise exception in Backend!

		}

	}
}