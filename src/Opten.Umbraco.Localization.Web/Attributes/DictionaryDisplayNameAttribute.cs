using Opten.Common.Extensions;
using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.ComponentModel;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// Localizes the Field from the Umbraco's Dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DictionaryDisplayNameAttribute : DisplayNameAttribute
	{

		private readonly string _dictionaryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryDisplayNameAttribute"/> class.
		/// </summary>
		/// <param name="dictionaryKey">The dictionary key.</param>
		public DictionaryDisplayNameAttribute(string dictionaryKey)
			: base(displayName: dictionaryKey)
		{
			_dictionaryKey = dictionaryKey;
		}

		/// <summary>
		/// Gets the display name for a property, event, or public void method that takes no arguments stored in this attribute.
		/// </summary>
		public override string DisplayName
		{
			get
			{
				string displayName = DictionaryHelper.GetDictionaryValue(key: _dictionaryKey);

				if (string.IsNullOrWhiteSpace(displayName))
				{
					return "[" + base.DisplayName + "]";
				}

				return displayName.NullCheckTrim();
			}
		}
	}
}
