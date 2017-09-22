using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	/// <summary>
	/// Helper to get the grid localized
	/// </summary>
	public static class GridTemplateExtensions
	{
		/// <summary>
		/// Gets the localized grid HTML.
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <param name="contentItem">The content item.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="framework">The framework.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static MvcHtmlString GetLocalizedGridHtml(this HtmlHelper html, IPublishedContent contentItem, string propertyAlias = "bodyText", string framework = "bootstrap3", string language = null)
		{
			return GetLocalizedGridHtml(contentItem, html, propertyAlias, framework);
		}

		/// <summary>
		/// Gets the localized grid HTML.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <param name="html">The HTML.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="framework">The framework.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException">No localized property type found with alias " + propertyAlias</exception>
		public static MvcHtmlString GetLocalizedGridHtml(this IPublishedContent contentItem, HtmlHelper html, string propertyAlias = "bodyText", string framework = "bootstrap3", string language = null)
		{
			Mandate.ParameterNotNullOrEmpty(propertyAlias, "propertyAlias");

			var view = "Grid/" + framework;
			var prop = contentItem.GetLocalizedProperty(propertyAlias, language, false, true);
			if (prop == null) throw new NullReferenceException("No localized property type found with alias " + propertyAlias);
			var model = prop.Value;

			var asString = model as string;
			if (asString != null && string.IsNullOrEmpty(asString)) return new MvcHtmlString(string.Empty);

			return html.Partial(view, model);
		}
	}
}
