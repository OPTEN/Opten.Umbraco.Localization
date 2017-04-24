using NUnit.Framework;
using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web;
using System.Globalization;
using System.Net.Http.Headers;

namespace Opten.Umbraco.Localization.Test.Extensions
{
	[TestFixture]
	public class RequestTests
	{

		//TODO: Do we need that? It's handled in the Opten.Common so we could remove the _cultures?

		[SetUp]
		public void Initialize()
		{
			LocalizationContext._cultures = new CultureInfo[] {
				new CultureInfo("de-CH"),
				new CultureInfo("fr-CH")
			};
		}

		[TearDown]
		public void TearDown()
		{
			LocalizationContext._cultures = null;
		}

		[Test]
		public void Accept_Language()
		{
			StringWithQualityHeaderValue[] header = new StringWithQualityHeaderValue[] {
				new StringWithQualityHeaderValue("fr", 1.0),
				new StringWithQualityHeaderValue("de", 0.5)
			};

			Assert.AreEqual("fr", header.TryGetCultureFromAcceptLanguage(LocalizationContext.Cultures).TwoLetterISOLanguageName);
		}

		[Test]
		public void Accept_Language_FireFox()
		{
			StringWithQualityHeaderValue[] header = new StringWithQualityHeaderValue[] {
				new StringWithQualityHeaderValue("fr"),
				new StringWithQualityHeaderValue("de", 0.5)
			};

			Assert.AreEqual("fr", header.TryGetCultureFromAcceptLanguage(LocalizationContext.Cultures).TwoLetterISOLanguageName);
			Assert.AreEqual("fr", "fr, de;q=0.5".ToAcceptLanguage().TryGetCultureFromAcceptLanguage(LocalizationContext.Cultures).TwoLetterISOLanguageName);
			Assert.AreEqual("fr", "de;q=0.5, fr".ToAcceptLanguage().TryGetCultureFromAcceptLanguage(LocalizationContext.Cultures).TwoLetterISOLanguageName);
		}

	}
}