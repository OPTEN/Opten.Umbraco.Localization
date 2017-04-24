using Opten.Common.Extensions;
using System;
using System.Text;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;

namespace Opten.Umbraco.Localization.Web.Routing
{
	/// <summary>
	/// The Content Finder for the Url Alias Provider.
	/// </summary>
	public class ContentFinderByUrlAlias : IContentFinder
	{

		//TODO: NUnit Tests: https://github.com/umbraco/Umbraco-CMS/blob/5397f2c53acbdeb0805e1fe39fda938f571d295a/src/Umbraco.Tests/Routing/ContentFinderByAliasWithDomainsTests.cs

		/// <summary>
		/// Tries to find and assign an Umbraco document to a <c>PublishedContentRequest</c>.
		/// </summary>
		/// <param name="contentRequest">The <c>PublishedContentRequest</c>.</param>
		/// <returns>
		/// A value indicating whether an Umbraco document was found and assigned.
		/// </returns>
		/// <remarks>
		/// Optionally, can also assign the template or anything else on the document request, although that is not required.
		/// </remarks>
		public bool TryFindContent(PublishedContentRequest contentRequest)
		{
			// Following helped to write this code:
			// ContentFinderByUrlAlias.cs
			// ContentFinderByNiceUrl.cs
			// ContentFinderByIdPath.cs

			// Code from:
			// https://github.com/umbraco/Umbraco-CMS/blob/master/src/Umbraco.Web/Routing/ContentFinderByUrlAlias.cs
			// https://github.com/umbraco/Umbraco-CMS/blob/master/src/Umbraco.Web/Routing/ContentFinderByNiceUrl.cs

			IPublishedContent content = null;

			if (contentRequest.Uri.AbsolutePath != "/") // no alias if "/"
			{
				content = FindContentByAlias(
						contentRequest.RoutingContext.UmbracoContext.ContentCache,
						contentRequest.HasDomain && contentRequest.UmbracoDomain.HasIdentity ?
							contentRequest.UmbracoDomain.RootContentId.Value :
							0,
						contentRequest.HasDomain ?
							DomainHelper.PathRelativeToDomain(contentRequest.DomainUri, contentRequest.Uri.GetAbsolutePathDecoded()) :
							contentRequest.Uri.GetAbsolutePathDecoded());

				if (content != null)
				{
					contentRequest.PublishedContent = content;
					LogHelper.Debug<ContentFinderByUrlAlias>("Path \"{0}\" is an alias for id={1}", () => contentRequest.Uri.AbsolutePath, () => contentRequest.PublishedContent.Id);
				}
			}

			return content != null && contentRequest.HasPublishedContent;
		}

		private IPublishedContent FindContentByAlias(ContextualPublishedContentCache cache, int rootContentId, string alias)
		{
			Mandate.ParameterNotNullOrEmpty(alias, "alias");

			StringBuilder xpathBuilder = new StringBuilder();
			bool hideTopLevelNodeFromPath = GlobalSettings.HideTopLevelNodeFromPath;

			// Build xPath

			if (rootContentId == 0)
			{
				if (hideTopLevelNodeFromPath)
					xpathBuilder.Append(XPathStringsDefinition.RootDocuments); // first node is not in the url 
				else
					xpathBuilder.Append(XPathStringsDefinition.Root);
			}
			else
			{
				xpathBuilder.Append(XPathStringsDefinition.Root);
				xpathBuilder.AppendFormat(XPathStringsDefinition.DescendantDocumentById, rootContentId);
				// always "hide top level" when there's a domain
			}

			// the alias may be "foo/bar" or "/foo/bar"
			// there may be spaces as in "/foo/bar,  /foo/nil"
			// these should probably be taken care of earlier on

			alias = alias.RemoveLeadingAndTrailingSlashAndBackslash();// .TrimStart('/');
			string[] urlNames = alias.Split(SlashChar, StringSplitOptions.RemoveEmptyEntries);

			// We don't care about the level anymore because we do it by child anyway

			string urlName;
			for (int i = 0; i < urlNames.Length; i++)
			{
				urlName = urlNames[i].RemoveLeadingAndTrailingSlashAndBackslash();

				// If the top level nodes are hidden the level is +1
				/*if (GlobalSettings.HideTopLevelNodeFromPath)
				{
					// First I thought this is problematic...
					// because we do not know if the backend user has overridden the urls e.g.
					// normal: /test1/test2 means that "/" is the first one (because of hide top level node from path)
					// overridden /test/test1/test2 so "/" is NOT the first one ("/" is overridden to "test")
					// but that's ok because we hide the textboxes in the root if it's HideTopLevelNodeFromPath
					// because then it makes no sense he overrides the url if it's "/"

					if (level == 1)
					{
						xPathBuilder.Append("/"); // Needs to be descendants because we start at level 2
					}

					level++;
				}*/

				xpathBuilder.AppendFormat(XPathStringsDefinition.ChildDocumentByUrlName,
										  Core.Constants.Conventions.Content.UrlAlias,
										  urlName);
			}

			//LogHelper.Info<ContentFinderByUrlAlias>("xPath: " + xpathBuilder.ToString());

			// xPath is like: /root//* [@isDoc and ((@urlName = 'frontpage' or langProperty = 'frontpage') and @level = 1)]/* [@isDoc and ((@urlName = 'textpage' or langProperty = 'textpage') and @level = 2)] ...
			return cache.GetSingleByXPath(xpathBuilder.ToString()/*, aliasVariable*/);
		}

		#region XPath Strings

		static readonly char[] SlashChar = new[] { '/' };

		private static class XPathStringsDefinition
		{
			public static string Root { get { return "/root"; } }

			public static string RootDocuments { get { return "/root/* [@isDoc]"; } }

			public static string DescendantDocumentById { get { return "//* [@isDoc and @id={0}]"; } }

			public static string ChildDocumentByUrlName
			{
				get
				{
					return "/* [@isDoc and (" +
						//"@level = {0}" +
						//" and " +
						//	"(" +
									"@urlName = '{1}'" +
									" or " +
									"contains({0}, \"{1}\")" +
						//	")" +
							")]";
				}
			}
		}

		#endregion
	}
}