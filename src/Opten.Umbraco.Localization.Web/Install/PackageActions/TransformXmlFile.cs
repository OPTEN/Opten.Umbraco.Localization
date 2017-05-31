using Microsoft.Web.XmlTransform;
using System;
using System.Web;
using System.Xml;
using umbraco.interfaces;

namespace Opten.Umbraco.Localization.Web.Install.PackageActions
{
	/// <summary>
	/// Package Action to transform an xml file
	/// </summary>
	/// <seealso cref="umbraco.interfaces.IPackageAction" />
	public class TransformXmlFile : IPackageAction
	{
		/// <summary>
		/// Alias of this PackageAction
		/// </summary>
		/// <returns></returns>
		public string Alias()
		{
			return "Opten.Umbraco.Localization.TransformXmlFile";
		}

		/// <summary>
		/// Executes the specified package name.
		/// </summary>
		/// <param name="packageName">Name of the package.</param>
		/// <param name="xmlData">The XML data.</param>
		/// <returns></returns>
		public bool Execute(string packageName, XmlNode xmlData)
		{
			return Transform(xmlData);
		}

		/// <summary>
		/// Sample of the XML.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public XmlNode SampleXml()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Undoes the specified package name.
		/// </summary>
		/// <param name="packageName">Name of the package.</param>
		/// <param name="xmlData">The XML data.</param>
		/// <returns></returns>
		public bool Undo(string packageName, XmlNode xmlData)
		{
			return Transform(xmlData, true);
		}

		private bool Transform(XmlNode xmlData, bool uninstall = false)
		{
			var file = xmlData.Attributes["file"].Value;
			var xdtfile = string.Format("{0}.{1}", xmlData.Attributes["xdtfile"].Value, (uninstall ? "un" : "") + "install.xdt");

			string sourceDocFileName = VirtualPathUtility.ToAbsolute(file);
			string xdtFileName = VirtualPathUtility.ToAbsolute(xdtfile);

			using (var xmlDoc = new XmlTransformableDocument())
			{
				xmlDoc.PreserveWhitespace = true;
				xmlDoc.Load(HttpContext.Current.Server.MapPath(sourceDocFileName));

				using (var xmlTrans = new XmlTransformation(HttpContext.Current.Server.MapPath(xdtFileName)))
				{
					if (xmlTrans.Apply(xmlDoc))
					{
						xmlDoc.Save(HttpContext.Current.Server.MapPath(sourceDocFileName));
					}
				}
			}
			return true;

		}
	}
}
