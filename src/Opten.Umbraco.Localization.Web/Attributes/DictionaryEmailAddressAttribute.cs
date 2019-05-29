using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// Validates the E-Mail-Address Field. The Error Message is resolved from the Umbraco's Dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class DictionaryEmailAddressAttribute : DataTypeAttribute, IClientValidatable
	{

		// https://github.com/Microsoft/referencesource/blob/master/System.ComponentModel.DataAnnotations/DataAnnotations/EmailAddressAttribute.cs
		// This attribute provides server-side email validation equivalent to jquery validate,
		// and therefore shares the same regular expression.
		private static Regex _regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
		private readonly string _dictionaryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryEmailAddressAttribute"/> class.
		/// </summary>
		/// <param name="dictionaryKey">The dictionary key.</param>
		public DictionaryEmailAddressAttribute(string dictionaryKey = "Validation Field EMail Invalid")
			: base(DictionaryHelper.GetDictionaryValue(key: dictionaryKey))
		{
			_dictionaryKey = dictionaryKey;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
			{
				return ValidationResult.Success;
			}

			string valueAsString = value as string;
			if (valueAsString != null && _regex.Match(valueAsString).Length > 0)
			{
				return ValidationResult.Success;
			}
			else
			{
				return new ValidationResult(
					_dictionaryKey,
					new[] { validationContext.MemberName });
			}
		}

		IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			ModelClientValidationRule rule = new ModelClientValidationRule
			{
				ErrorMessage = GetErrorMessage(metadata.GetDisplayName()),
				ValidationType = "email",
			};

			rule.ValidationParameters.Add("pattern", _regex);

			yield return rule;
		}

		private string GetErrorMessage(string displayName)
		{
			var errorMessage = string.Format(DictionaryHelper.GetDictionaryValue(_dictionaryKey), displayName);

			if (string.IsNullOrWhiteSpace(errorMessage))
			{
				errorMessage = "[" + _dictionaryKey + "]";
			}

			this.ErrorMessage = errorMessage;

			return errorMessage;
		}
	}
}
