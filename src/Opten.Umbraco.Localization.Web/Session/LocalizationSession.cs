using Opten.Umbraco.Localization.Web.Application;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models = Opten.Umbraco.Localization.Core.Models;

namespace Opten.Umbraco.Localization.Web.Session
{
	public class LocalizationSession
	{

		private const string _sessionKey = "OptenUmbracoLocalizationSession";

		public IEnumerable<Models.Language> SelectedLanguages { get; set; }

		public IEnumerable<Models.Language> Languages { get; set; }

		public LocalizationSession()
		{
			this.Languages = this.GetAllLanguages();
		}

		public static LocalizationSession Current
		{
			get
			{
				LocalizationSession session = null;
				if (HttpContext.Current.Session != null)
				{
					session = (LocalizationSession)HttpContext.Current.Session[_sessionKey];
				}
				
				if (session == null)
				{
					session = new LocalizationSession();
					HttpContext.Current.Session[_sessionKey] = session;
				}

				return session;
			}
		}

		private List<Models.Language> GetAllLanguages()
		{
			List<Models.Language> languages = new List<Models.Language>();

			languages.AddRange(
				Localizer.Languages
				.Select(o => o.CultureInfo)
				.Select(o => new Models.Language
				{
					ISOCode = o.Name,
					DisplayName = o.DisplayName,
					NativeName = o.NativeName
				}));

			return languages;
		}
	}
}
