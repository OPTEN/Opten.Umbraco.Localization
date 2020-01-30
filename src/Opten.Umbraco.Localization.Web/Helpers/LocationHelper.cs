using Newtonsoft.Json;
using Opten.Umbraco.Localization.Web.Helpers;
using Opten.Umbraco.Localization.Web.Models;
using Opten.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using Umbraco.Core.Logging;
using Umbraco.Core;

namespace Opten.Umbraco.Localization.Web.Controllers
{

	public class LocationHelper
	{
		private static bool UseTestApi = false;
		private static string TestApiUrl = "http://api.ipstack.com/";
		private static string ApiUrl = "https://api.ipstack.com/";
		private static string ApiKey = string.Empty;
		private static string[] ExcludedIps = null;

		private static List<string> Crawlers = new List<string>()
		{
			"bot","crawler","spider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google",
			"lwp-trivial","nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne",
			"atn_worldwide","atomz","bjaaland","ukonline","calif","combine","cosmos","cusco",
			"cyberspyder","digger","grabber","downloadexpress","ecollector","ebiness","esculapio",
			"esther","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
			"gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","havindex","hotwired",
			"htdig","ingrid","informant","inspectorwww","iron33","teoma","ask jeeves","jeeves",
			"image.kapsi.net","kdd-explorer","label-grabber","larbin","linkidator","linkwalker",
			"lockon","marvin","mattie","mediafox","merzscope","nec-meshexplorer","udmsearch","moget",
			"motor","muncher","muninn","muscatferret","mwdsearch","sharp-info-agent","webmechanic",
			"netscoop","newscan-online","objectssearch","orbsearch","packrat","pageboy","parasite",
			"patric","pegasus","phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus",
			"raven","roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au",
			"searchprocess","senrigan","shagseeker","site valet","skymob","slurp","snooper","speedy",
			"curl_image_client","suke","www.sygol.com","tach_bw","templeton","titin","topiclink","udmsearch",
			"urlck","valkyrie libwww-perl","verticrawl","victoria","webscout","voyager","crawlpaper",
			"webcatcher","t-h-u-n-d-e-r-s-t-o-n-e","webmoose","pagesinventory","webquest","webreaper",
			"webwalker","winona","occam","robi","fdse","jobo","rhcs","gazz","dwcp","yeti","fido","wlm",
			"wolp","wwwc","xget","legs","curl","webs","wget","sift","cmc"
		};

		static LocationHelper()
		{
			Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:ipstack:useTestApi"], out UseTestApi);
			ApiKey = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:apiKey");
			ExcludedIps = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:excludedIps")?.ConvertCommaSeparatedToStringArray();
		}

		public static RegionInfo GetRegionInfoByIPAddress(string ipAddress, bool useCookie = true)
		{
			IPStackResponse location = GetLocationByIPAddress(ipAddress, useCookie);
			if (location != null && string.IsNullOrWhiteSpace(location.CountryCode) == false)
			{
				return new RegionInfo(location.CountryCode);
			}
			return null;
		}

		public static RegionInfo GetRegionInfoByCurrentIPAddress()
		{
			IPStackResponse location = GetLocationByIPAddress(IPAddressHelper.GetIPAddress());
			if (location != null && string.IsNullOrWhiteSpace(location.CountryCode) == false)
			{
				return new RegionInfo(location.CountryCode);
			}
			return null;
		}

		public static IPStackResponse GetLocationByIPAddress(string ipAddress, bool useCookie = true)
		{
			try
			{
				if (IsExcludedIp(ipAddress) || IsRequestFromCrawler())
				{
					return null;
				}
				if (HttpContext.Current.Request.RawUrl.Contains("umbraco/backoffice") == false)
				{
					if (useCookie)
					{
						HttpCookie cookie = HttpContext.Current.Request.Cookies[Core.Constants.Cookie.Country];
						if (cookie != null && string.IsNullOrWhiteSpace(cookie.Value) == false)
						{
							return new IPStackResponse() { CountryCode = cookie.Value };
						}
					}
					return (IPStackResponse)ApplicationContext.Current.ApplicationCache.IsolatedRuntimeCache.GetOrCreateCache<IPStackResponse>().GetCacheItem($"{ipAddress}", () =>
					{
						using (WebClient wc = new WebClient())
						{
							string json = wc.DownloadString(GetRequestUrl(ipAddress));
							var ipStackResponse = JsonConvert.DeserializeObject<IPStackResponse>(json);
							if (string.IsNullOrWhiteSpace(ipStackResponse.CountryCode) == false)
							{
								if (useCookie)
								{
									UpdateCookie(ipStackResponse);
								}
								LogHelper.Info<LocationHelper>($"Request Region Data by following IP: {ipAddress} | URL: {HttpContext.Current.Request.RawUrl} | UserAgent: {HttpContext.Current.Request.UserAgent}");
								return ipStackResponse;
							}
						}
						return null;
					});
				}
			}
			catch (Exception exc)
			{
				LogHelper.Error<LocationHelper>($"Could not get Region Data by following IP: {ipAddress}", exc);
			}
			return null;
		}

		private static bool IsExcludedIp(string ipAddress)
		{
			if (ExcludedIps == null || ExcludedIps.Any() == false) {
				return false;
			}
			return ExcludedIps.Contains(ipAddress) || ExcludedIps.Any(o => ipAddress.StartsWith(o));
		}

		private static bool IsRequestFromCrawler()
		{
			string ua = HttpContext.Current.Request.UserAgent?.ToLower();
			if (string.IsNullOrWhiteSpace(ua))
			{
				return true;
			}
			return Crawlers.Exists(x => ua.Contains(x));
		}

		private static string GetRequestUrl(string ipAddress, string responseLanguage = "en")
		{
			return $"{(UseTestApi ? TestApiUrl : ApiUrl)}{ipAddress}?access_key={ApiKey}&output=json";
		}

		private static void UpdateCookie(IPStackResponse ipStackResponse)
		{
			if (ipStackResponse != null)
			{
				HttpCookie cookie = new HttpCookie(Core.Constants.Cookie.Country);
				cookie.Expires = DateTime.Now.AddDays(1);
				cookie.Path = "/";
				cookie.Value = ipStackResponse.CountryCode;
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
		}
	}
}
