using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// Validates the required Field. The Error Message is resolved from the Umbraco's Dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DictionaryRequiredAttribute : RequiredAttribute, IClientValidatable
	{

		private readonly string _dictionaryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryRequiredAttribute"/> class.
		/// </summary>
		/// <param name="dictionaryKey">The dictionary key.</param>
		public DictionaryRequiredAttribute(string dictionaryKey = "Validation Field Required")
		{
			_dictionaryKey = dictionaryKey;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (this.IsValid(value) == false)
			{
				return new ValidationResult(
					GetErrorMessage(validationContext.DisplayName),
					new[] { validationContext.MemberName });
			}
			else return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			string displayName = metadata.GetDisplayName();

			yield return new ModelClientValidationRule
			{
				ErrorMessage = GetErrorMessage(displayName),
				ValidationType = "required",
			};
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
