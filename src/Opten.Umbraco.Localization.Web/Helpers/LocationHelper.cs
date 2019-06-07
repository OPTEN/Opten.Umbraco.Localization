using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;
using System.Net;

namespace Opten.Umbraco.Localization.Web.Helpers
{
	public static class LocationHelper
	{
		private static string ApiUrl = string.Empty;
		private static string ApiKey = string.Empty;

		static LocationHelper()
		{
			ApiKey = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:apiKey");
			ApiUrl = $"http://api.ipstack.com/{0}?access_key={ApiKey}&output=json";
		}

		public static RegionInfo GetRegionInfoByIPAddress()
		{
			IPStackResponse location = GetLocationByIPAddress();
			if (location != null && string.IsNullOrWhiteSpace(location.CountryName) == false)
			{
				return new RegionInfo(location.CountryName);
			}
			return null;
		}

		public static IPStackResponse GetLocationByIPAddress()
		{
			using (WebClient wc = new WebClient())
			{
				var json = wc.DownloadString(GetRequestUrl(IPAddressHelper.GetIPAddress()));
				return JsonConvert.DeserializeObject<IPStackResponse>(json);
			}
		}

		private static string GetRequestUrl(string ipAddress)
		{
			return string.Format(ApiUrl, ipAddress);
		}
	}

	public class IPStackResponse
	{
		[JsonProperty("ip")]
		public string IP { get; set; }

		[JsonProperty("country_code")]
		public string CountryCode { get; set; }

		[JsonProperty("country_name")]
		public string CountryName { get; set; }
	}
}
