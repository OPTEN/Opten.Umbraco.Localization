using System.Globalization;

namespace Opten.Umbraco.Localization.Web.Extensions
{
	public static class StringExtensions
	{
		public static string ForceEndsWith(this string input, char value)
		{
			return input.EndsWith(value.ToString(CultureInfo.InvariantCulture)) ? input : input + value;
		}
	}
}
