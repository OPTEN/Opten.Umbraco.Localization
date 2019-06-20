using Newtonsoft.Json;
using Opten.Umbraco.Localization.Web.Helpers;
using Opten.Umbraco.Localization.Web.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Web;

namespace Opten.Umbraco.Localization.Web.Controllers
{
	public class LocationApiController
	{
		private static bool UseTestApi = false;
		private static string TestApiUrl = "http://api.ipstack.com/";
		private static string ApiUrl = "https://api.ipstack.com/";
		private static string ApiKey = string.Empty;

		static LocationApiController()
		{
			Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:ipstack:useTestApi"], out UseTestApi);
			ApiKey = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:apiKey");
		}

		public RegionInfo GetRegionInfoByIPAddress(string ipAddress, bool useCookie = true)
		{
			IPStackResponse location = GetLocationByIPAddress(ipAddress, useCookie);
			if (location != null && string.IsNullOrWhiteSpace(location.CountryCode) == false)
			{
				return new RegionInfo(location.CountryCode);
			}
			return null;
		}

		public RegionInfo GetRegionInfoByCurrentIPAddress()
		{
			IPStackResponse location = GetLocationByIPAddress(IPAddressHelper.GetIPAddress());
			if (location != null && string.IsNullOrWhiteSpace(location.CountryCode) == false)
			{
				return new RegionInfo(location.CountryCode);
			}
			return null;
		}

		public IPStackResponse GetLocationByIPAddress(string ipAddress, bool useCookie = true)
		{
			IPStackResponse ipStackResponse = null;
			if (HttpContext.Current.Request.RawUrl.Contains("umbraco/backoffice") == false)
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
					string json = wc.DownloadString(GetRequestUrl(ipAddress));
					ipStackResponse = JsonConvert.DeserializeObject<IPStackResponse>(json);
					if (useCookie && ipStackResponse.CountryCode != null)
					{
						UpdateCookie(ipStackResponse);
					}
				}
			}
			return ipStackResponse;

		}

		private string GetRequestUrl(string ipAddress, string responseLanguage = "en")
		{
			return $"{(UseTestApi ? TestApiUrl : ApiUrl)}{ipAddress}?access_key={ApiKey}&output=json";
		}

		private void UpdateCookie(IPStackResponse ipStackResponse)
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
}
