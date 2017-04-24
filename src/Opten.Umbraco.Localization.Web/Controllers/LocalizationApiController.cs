using Opten.Umbraco.Localization.Web.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
						DisplayName = o.DisplayName,
						NativeName = o.NativeName
					}));

			// See if one has already been set via the event handler
			Models.Language activeLanguage = languages.FirstOrDefault(o => o.IsDefault);

			// If not try set from current thread
			if (activeLanguage == null)
				activeLanguage = GetActiveLanguage(languages, Thread.CurrentThread.CurrentUICulture.Name);

			// Couldn't find from the thread, so try from web.config
			if (activeLanguage == null)
				activeLanguage = GetActiveLanguage(languages, LocalizationContext.DefaultCulture.Name);

			// Couldn't find a good enough match, just select the first language
			if (activeLanguage == null)
				activeLanguage = languages.FirstOrDefault();

			// If we found any we set it as the default language for the backend
			if (activeLanguage != null)
				languages.Single(o => o.ISOCode == activeLanguage.ISOCode).IsDefault = true;

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
			HttpCookie cookie = HttpContext.Current.Request.Cookies[Core.Constants.Cache.BackendLanguages];

			// This is equivalent to all languages or default language
			if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
				return Enumerable.Empty<Models.Language>();

			// otherwise something was changed/cached
			IEnumerable<ILanguage> languages = LocalizationContext.CurrentBackEndUserLanguages();

			return languages.Select(o => o.CultureInfo)
							.Select(o => new Models.Language
							{
								ISOCode = o.Name,
								DisplayName = o.DisplayName,
								NativeName = o.NativeName
							});
		}

		public bool PostSelectedLanguages(string[] languages)
		{
			HttpCookie cookie = new HttpCookie(Core.Constants.Cache.BackendLanguages);
			cookie.Expires = DateTime.Now.AddDays(1);
			cookie.Path = "/";

			string[] languagesToStore;
			if (languages == null || languages.Any() == false)
			{
				languagesToStore = LocalizationContext.Languages.Select(o => o.IsoCode)
													  .ToArray();
			}
			else
			{
				languagesToStore = LocalizationContext.Languages.Where(o => languages.Contains(o.IsoCode))
													  .Select(o => o.IsoCode)
													  .ToArray();
			}

			cookie.Value = string.Join(",", languagesToStore);

			HttpContext.Current.Response.Cookies.Add(cookie);

			return true;
		}

		#region Private helpers

		private Models.Language GetActiveLanguage(
			List<Models.Language> languages,
			string isoCode)
		{
			// Try settings to exact match of current culture
			Models.Language language = languages.FirstOrDefault(o => o.ISOCode == isoCode);

			// Try setting to nearest match
			if (language == null)
				language = languages.FirstOrDefault(o => o.ISOCode.Contains(isoCode));

			// Try setting to nearest match
			if (language == null)
				language = languages.FirstOrDefault(o => isoCode.Contains(o.ISOCode));

			return language;
		}

		#endregion

	}

#pragma warning restore 1591

}