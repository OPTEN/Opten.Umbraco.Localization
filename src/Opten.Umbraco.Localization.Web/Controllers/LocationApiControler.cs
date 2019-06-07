using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Web;

namespace Opten.Umbraco.Localization.Web.Helpers
{
	public static class LocationApiControler
	{
		private static bool UseTestApi = false;
		private static string TestApiUrl = "http://api.ipstack.com/";
		private static string ApiUrl = "https://api.ipstack.com/";
		private static string ApiKey = string.Empty;

		static LocationApiControler()
		{
			Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:ipstack:useTestApi"], out UseTestApi);
			ApiKey = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:apiKey");
		}

		public static RegionInfo GetRegionInfoByCurrentIPAddress()
		{
			IPStackResponse location = GetLocationByIPAddress(IPAddressHelper.GetIPAddress());
			if (location != null && string.IsNullOrWhiteSpace(location.CountryName) == false)
			{
				return new RegionInfo(location.CountryName);
			}
			return null;
		}

		public static IPStackResponse GetLocationByIPAddress(string ipAddress, bool useCookie = true)
		{
			if (useCookie)
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[Core.Constants.Cache.Country];
				if (cookie != null && string.IsNullOrWhiteSpace(cookie.Value) == false)
				{
					return new IPStackResponse() { CountryCode = cookie.Value };
				}
			}
			using (WebClient wc = new WebClient())
			{
				var json = wc.DownloadString(GetRequestUrl(ipAddress));
				var ipStackResponse = JsonConvert.DeserializeObject<IPStackResponse>(json);
				UpdateCookie(ipStackResponse);
				return ipStackResponse;
			}
		}

		private static string GetRequestUrl(string ipAddress, string responseLanguage = "en")
		{
			return $"{(UseTestApi ? TestApiUrl : ApiUrl)}{ipAddress}?access_key={ApiKey}&output=json";
		}

		private static void UpdateCookie(IPStackResponse ipStackResponse)
		{
			if (ipStackResponse != null)
			{
				HttpCookie cookie = new HttpCookie(Core.Constants.Cache.Country);
				cookie.Expires = DateTime.Now.AddDays(1);
				cookie.Path = "/";
				cookie.Value = ipStackResponse.CountryCode;
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
		}
	}

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
