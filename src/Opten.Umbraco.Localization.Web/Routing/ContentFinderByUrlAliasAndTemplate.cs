using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	/// <summary>
	/// Provides an implementation of <see cref="IContentFinder"/> that handles page nice urls, url alias and a template.
	/// </summary>
	/// <remarks>
	/// <para>Handles <c>/foo/bar/template</c> where <c>/foo/bar</c> is the nice url or url alias of a document, and <c>template</c> a template alias.</para>
	/// <para>If successful, then the template of the document request is also assigned.</para>
	/// </remarks>
	public class ContentFinderByUrlAliasAndTemplate : ContentFinderByUrlAlias
	{
		/// <summary>
		/// Tries to find and assign an Umbraco document to a <c>PublishedContentRequest</c>.
		/// </summary>
		/// <param name="contentRequest">The <c>PublishedContentRequest</c>.</param>		
		/// <returns>A value indicating whether an Umbraco document was found and assigned.</returns>
		/// <remarks>If successful, also assigns the template.</remarks>
		public override bool TryFindContent(PublishedContentRequest contentRequest)
		{
			IPublishedContent node = null;
			string path = contentRequest.Uri.GetAbsolutePathDecoded();

			if (contentRequest.HasDomain)
				path = DomainHelper.PathRelativeToDomain(contentRequest.DomainUri, path);

			if (path != "/") // no template if "/"
			{
				var pos = path.LastIndexOf('/');
				var templateAlias = path.Substring(pos + 1);
				path = pos == 0 ? "/" : path.Substring(0, pos);

				var template = ApplicationContext.Current.Services.FileService.GetTemplate(templateAlias);
				if (template != null)
				{
					LogHelper.Debug<ContentFinderByUrlAliasAndTemplate>("Valid template: \"{0}\"", () => templateAlias);

					node = FindContentByAlias(
						contentRequest.RoutingContext.UmbracoContext.ContentCache,
						contentRequest.HasDomain && contentRequest.UmbracoDomain.HasIdentity ?
							contentRequest.UmbracoDomain.RootContentId.Value :
							0,
						contentRequest.HasDomain ?
							DomainHelper.PathRelativeToDomain(contentRequest.DomainUri, contentRequest.Uri.GetAbsolutePathDecoded()) :
							contentRequest.Uri.GetAbsolutePathDecoded());

					if (UmbracoConfig.For.UmbracoSettings().WebRouting.DisableAlternativeTemplates == false && node != null)
						contentRequest.SetTemplate(template);
				}
				else
				{
					LogHelper.Debug<ContentFinderByUrlAliasAndTemplate>("Not a valid template: \"{0}\"", () => templateAlias);
				}
			}
			else
			{
				LogHelper.Debug<ContentFinderByUrlAliasAndTemplate>("No template in path \"/\"");
			}

			return node != null;
		}
	}
}
