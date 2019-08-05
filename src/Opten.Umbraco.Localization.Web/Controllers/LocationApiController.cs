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

namespace Opten.Umbraco.Localization.Web.Controllers
{

	public class LocationApiController
	{
		private static bool UseTestApi = false;
		private static string TestApiUrl = "http://api.ipstack.com/";
		private static string ApiUrl = "https://api.ipstack.com/";
		private static string ApiKey = string.Empty;
		private static string[] ExcludedIps = null;
		private static List<IPStackRequest> Requests = new List<IPStackRequest>();

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

		static LocationApiController()
		{
			Boolean.TryParse(ConfigurationManager.AppSettings["OPTEN:localization:ipstack:useTestApi"], out UseTestApi);
			ApiKey = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:apiKey");
			ExcludedIps = ConfigurationManager.AppSettings.Get("OPTEN:localization:ipstack:excludedIps").ConvertCommaSeparatedToStringArray();
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
			try
			{
				if (IsRequestFromCrawler() || IsExcludedIp(ipAddress))
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
					IPStackRequest firstSameRequest = Requests.FirstOrDefault(o => o.IP.Equals(ipAddress, StringComparison.OrdinalIgnoreCase) && o.Date > DateTime.Now.AddDays(-1));
					if (firstSameRequest != null)
					{
						return new IPStackResponse() { CountryCode = firstSameRequest.CountryCode };
					}
					using (WebClient wc = new WebClient())
					{
						string json = wc.DownloadString(GetRequestUrl(ipAddress));
						ipStackResponse = JsonConvert.DeserializeObject<IPStackResponse>(json);
						if (useCookie && ipStackResponse.CountryCode != null)
						{
							UpdateCookie(ipStackResponse);
							Requests.Add(new IPStackRequest(ipAddress, ipStackResponse.CountryCode));
						}
					}
				}
			}
			catch (Exception exc)
			{
				LogHelper.Error<LocationApiController>($"Could not get Region Data by following IP: {ipAddress}", exc);
			}
			return ipStackResponse;

		}

		private bool IsExcludedIp(string ipAddress)
		{
			return ExcludedIps.Contains(ipAddress);
		}

		private bool IsRequestFromCrawler()
		{
			string ua = HttpContext.Current.Request.UserAgent.ToLower();
			return Crawlers.Exists(x => ua.Contains(x));
		}

		private string GetRequestUrl(string ipAddress, string responseLanguage = "en")
		{
			return $"{(UseTestApi ? TestApiUrl : ApiUrl)}{ipAddress}?access_key={ApiKey}&output=json";
		}

		private void UpdateCookie(IPStackResponse ipStackResponse)
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
