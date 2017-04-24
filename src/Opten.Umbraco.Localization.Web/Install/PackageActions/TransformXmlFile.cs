using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using umbraco.interfaces;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using System.Web;
using Microsoft.Web.XmlTransform;

namespace Opten.Umbraco.Localization.Web.Install.PackageActions
{
	/// <summary>
	/// Package Action to transform an xml file
	/// </summary>
	/// <seealso cref="umbraco.interfaces.IPackageAction" />
	public class TransformXmlFile : IPackageAction
	{

		public string Alias()
		{
			return "Opten.Umbraco.Localization.TransformXmlFile";
		}

		public bool Execute(string packageName, XmlNode xmlData)
		{
			return Transform(xmlData);
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

		public XmlNode SampleXml()
		{
			return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true\" alias=\"Opten.Umbraco.Localization.TransformXmlFile\" file=\"~/web.config\" xdtfile=\"~/app_plugins/OPTEN.Localization/install/web.config\"></Action>");
		}

		public bool Undo(string packageName, XmlNode xmlData)
		{
			return Transform(xmlData, true);
		}
	}
}
