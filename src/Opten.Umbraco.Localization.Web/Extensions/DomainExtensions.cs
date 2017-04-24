using System;
using System.Globalization;
using System.Linq;
using Umbraco.Core.Models;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// The Domain Extensions.
	/// </summary>
	public static class DomainExtensions
	{

		/// <summary>
		/// Tries to get the culture by the domains.
		/// </summary>
		/// <param name="domains">The domains.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		public static CultureInfo TryGetCulture(
			this IDomain[] domains,
			CultureInfo culture)
		{
			if (domains == null || domains.Any() == false) return LocalizationContext.DefaultCulture;

			if (domains.Any(o => o.LanguageIsoCode.Equals(culture.Name, StringComparison.OrdinalIgnoreCase)) == false)
			{
				culture = LocalizationContext.DefaultCulture;
			}

			return culture;
		}

		/// <summary>
		/// Tries to the get culture by the domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		public static CultureInfo TryGetCulture(
			this IDomain domain,
			CultureInfo culture)
		{
			if (domain == null) return LocalizationContext.DefaultCulture;

			if (domain.LanguageIsoCode.Equals(culture.Name, StringComparison.OrdinalIgnoreCase) == false)
			{
				culture = LocalizationContext.DefaultCulture;
			}

			return culture;
		}

	}
}