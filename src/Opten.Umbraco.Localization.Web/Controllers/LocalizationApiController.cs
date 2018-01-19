using Opten.Umbraco.Localization.Web.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Models = Opten.Umbraco.Localization.Core.Models;

namespace Opten.Umbraco.Localization.Web.Controllers
{

#pragma warning disable 1591

	[PluginController("OPTEN"), IsBackOffice]
	public sealed class LocalizationApiController : UmbracoAuthorizedJsonController
	{

		private readonly Lazy<RoutingHelper> _routingHelper;

		public LocalizationApiController()
		{
			_routingHelper = new Lazy<RoutingHelper>(() => new RoutingHelper(
				domainService: base.Services.DomainService));
		}

		public IEnumerable<Models.Language> GetAllLanguages()
		{
			List<Models.Language> languages = new List<Models.Language>();

			languages.AddRange(
					LocalizationContext.Languages
					.Select(o => o.CultureInfo)
					.Select(o => new Models.Language
					{
						ISOCode = o.Name,
						Name = o.EnglishName
					}));

			if (languages.Any(o => o.IsDefault) == false)
			{
				ILanguage defaultLanguage = LocalizationContext.GetDefaultLanguage(LocalizationContext.Languages);
				languages.Single(o => o.ISOCode == defaultLanguage.CultureInfo.Name).IsDefault = true;
			}

			return languages;
		}

		public dynamic GetState(int contentId)
		{
			bool hasDomains = false;

			if (contentId > 0)
			{
				hasDomains = _routingHelper.Value.NodeHasDomains(
					contentId: contentId);
			}

			return new
			{
				isEnabled = Opten.Umbraco.Localization.Web.Routing.AliasUrlProvider.FindByUrlAliasEnabled,
				hasHostnames = hasDomains
			};
		}

		public string GetUrlSegment(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return string.Empty;

			// Somehow there is a problem: it converts ü to ue but Ü not, so we lowercase it
			// TODO: Tell umbraco?
			name = name.ToLowerInvariant();

			return name.ToUrlSegment();
		}

		public bool GetUrlAvailability(int contentId, string url, int level)
		{
			IEnumerable<IPublishedContent> contents = Enumerable.Empty<IPublishedContent>();

			// Try find an url in the same level
			string xpath = string.Format(CultureInfo.InvariantCulture, "/root//* [@isDoc and @id={0}]", contentId);
			IPublishedContent content = Umbraco.TypedContentSingleAtXPath(xpath: xpath);

			if (content.Level == level)
			{
				contents = content.Parent == null ? Enumerable.Empty<IPublishedContent>() : content.Parent.Children;
			}
			else
			{
				xpath = string.Format(CultureInfo.InvariantCulture, xpath + "/* [@isDoc and @level = {0}]", level);
				contents = Umbraco.TypedContentAtXPath(xpath);
			}

			// Except current node
			contents = contents.Where(o => o.Id != contentId).ToList();

			foreach (IPublishedContent node in contents)
			{
				if (_routingHelper.Value.HasUrlName(content: node, urlName: url))
				{
					return false;
				}
			}

			return true;
		}

		public IEnumerable<Models.Language> GetSelectedLanguages()
		{
			//TODO: check if there is any? because maybe the cookie lived long and some languages got deleted?
			// otherwise something was changed/cached
			return LocalizationContext.CurrentBackEndUserLanguages()
				.Select(o => o.CultureInfo)
				.Select(o => new Models.Language
				{
					ISOCode = o.Name,
					Name = o.EnglishName
				});
		}

		public bool PostSelectedLanguages(string[] languages)
		{
			HttpCookie cookie = new HttpCookie(Core.Constants.Cache.BackendLanguages);
			cookie.Expires = DateTime.Now.AddDays(1);
			cookie.Path = "/";

			string[] languagesToStore = LocalizationContext.Languages.Where(o => languages.Contains(o.IsoCode))
													  .Select(o => o.IsoCode)
													  .ToArray();

			cookie.Value = string.Join(",", languagesToStore);

			HttpContext.Current.Response.Cookies.Add(cookie);

			return true;
		}

	}

#pragma warning restore 1591

}