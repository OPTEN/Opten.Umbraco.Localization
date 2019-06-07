namespace Opten.Umbraco.Localization.Web.Helpers
{
	public static class IPAddressHelper
	{
		public static string GetIPAddress()
		{
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			if (string.IsNullOrEmpty(ipAddress) == false)
			{
				string[] addresses = ipAddress.Split(',');
				if (addresses.Length != 0)
				{
					return addresses[0];
				}
			}

			return context.Request.ServerVariables["REMOTE_ADDR"];
		}
	}
}
