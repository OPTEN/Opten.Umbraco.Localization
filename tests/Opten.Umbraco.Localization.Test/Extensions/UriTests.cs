using NUnit.Framework;
using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web;
using System;
using System.Globalization;

namespace Opten.Umbraco.Localization.Test.Extensions
{
	[TestFixture]
	public class UriTests
	{

		private readonly Uri uri = new Uri("http://www.opten.ch/de/?test=1&test2=2");

		[SetUp]
		public void Initialize()
		{
			LocalizationContext._cultures = new CultureInfo[] {
				new CultureInfo("de-CH")
			};
		}

		[TearDown]
		public void TearDown()
		{
			LocalizationContext._cultures = null;
		}

		#region Without Language

		[Test]
		public void Get_Url_Without_Query_Without_Language()
		{
			Assert.AreEqual("http://www.opten.ch/", uri.GetUrlWithoutLanguage(withQuery: false, withDomain: true));
			Assert.AreEqual("/", uri.GetUrlWithoutLanguage(withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language()
		{
			Assert.AreEqual("http://www.opten.ch/?test=1&test2=2", uri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test=1&test2=2", uri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_Without_Query_Without_Language_And_Add_Params()
		{
			Uri newUri = uri.AddQueryParam("test3", "3");
			Assert.AreEqual("http://www.opten.ch/", newUri.GetUrlWithoutLanguage(withQuery: false, withDomain: true));
			Assert.AreEqual("/", newUri.GetUrlWithoutLanguage(withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Add_Params()
		{
			Uri newUri = uri.AddQueryParam("test3", "3");
			Assert.AreEqual("http://www.opten.ch/?test=1&test2=2&test3=3", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test=1&test2=2&test3=3", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_Without_Query_Without_Language_And_Remove_First_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test1");
			Assert.AreEqual("http://www.opten.ch/", newUri.GetUrlWithoutLanguage(withQuery: false, withDomain: true));
			Assert.AreEqual("/", newUri.GetUrlWithoutLanguage(withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Remove_First_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test");
			Assert.AreEqual("http://www.opten.ch/?test2=2", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test2=2", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Remove_Second_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test2");
			Assert.AreEqual("http://www.opten.ch/?test=1", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test=1", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Remove_NonExisting_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test3");
			Assert.AreEqual("http://www.opten.ch/?test=1&test2=2", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test=1&test2=2", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Remove_All_Params()
		{
			Uri newUri = uri.RemoveQueryParam("test").RemoveQueryParam("test2");
			Assert.AreEqual("http://www.opten.ch/", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Update_First_Param()
		{
			Uri newUri = uri.UpdateQueryParam("test", "test");
			Assert.AreEqual("http://www.opten.ch/?test2=2&test=test", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test2=2&test=test", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Update_First_Param_Ignore_Case()
		{
			Uri newUri = uri.UpdateQueryParam("TEST", "test");
			Assert.AreEqual("http://www.opten.ch/?test2=2&TEST=test", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test2=2&TEST=test", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_Without_Language_And_Update_NonExisting_Param()
		{
			Uri newUri = uri.UpdateQueryParam("test4", "test4");
			Assert.AreEqual("http://www.opten.ch/?test=1&test2=2&test4=test4", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: true));
			Assert.AreEqual("/?test=1&test2=2&test4=test4", newUri.GetUrlWithoutLanguage(withQuery: true, withDomain: false));
		}

		#endregion
		
		#region With Language

		[Test]
		public void Get_Url_Without_Query_With_Language()
		{
			Assert.AreEqual("http://www.opten.ch/de/", uri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: true));
			Assert.AreEqual("/de/", uri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language()
		{
			Assert.AreEqual("http://www.opten.ch/de/?test=1&test2=2", uri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test=1&test2=2", uri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_Without_Query_With_Language_And_Add_Params()
		{
			Uri newUri = uri.AddQueryParam("test3", "3");
			Assert.AreEqual("http://www.opten.ch/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: true));
			Assert.AreEqual("/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Add_Params()
		{
			Uri newUri = uri.AddQueryParam("test3", "3");
			Assert.AreEqual("http://www.opten.ch/de/?test=1&test2=2&test3=3", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test=1&test2=2&test3=3", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_Without_Query_With_Language_And_Remove_First_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test1");
			Assert.AreEqual("http://www.opten.ch/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: true));
			Assert.AreEqual("/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: false, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Remove_First_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test");
			Assert.AreEqual("http://www.opten.ch/de/?test2=2", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test2=2", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Remove_Second_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test2");
			Assert.AreEqual("http://www.opten.ch/de/?test=1", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test=1", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Remove_NonExisting_Param()
		{
			Uri newUri = uri.RemoveQueryParam("test3");
			Assert.AreEqual("http://www.opten.ch/de/?test=1&test2=2", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test=1&test2=2", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Remove_All_Params()
		{
			Uri newUri = uri.RemoveQueryParam("test").RemoveQueryParam("test2");
			Assert.AreEqual("http://www.opten.ch/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Update_First_Param()
		{
			Uri newUri = uri.UpdateQueryParam("test", "test");
			Assert.AreEqual("http://www.opten.ch/de/?test2=2&test=test", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test2=2&test=test", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Update_First_Param_Ignore_Case()
		{
			Uri newUri = uri.UpdateQueryParam("TEST", "test");
			Assert.AreEqual("http://www.opten.ch/de/?test2=2&TEST=test", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test2=2&TEST=test", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		[Test]
		public void Get_Url_With_Query_With_Language_And_Update_NonExisting_Param()
		{
			Uri newUri = uri.UpdateQueryParam("test4", "test4");
			Assert.AreEqual("http://www.opten.ch/de/?test=1&test2=2&test4=test4", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: true));
			Assert.AreEqual("/de/?test=1&test2=2&test4=test4", newUri.GetUrlWithLanguage(language: "de", withQuery: true, withDomain: false));
		}

		#endregion

	}
}