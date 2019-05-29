using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// Validates the Field string-length. The Error Message is resolved from the Umbraco's Dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DictionaryPasswordAttribute : ValidationAttribute, IClientValidatable
	{

		//TODO: Add minRequiredNonalphanumericCharacters as error message

		private readonly string _dictionaryKey;

		/// <summary>
		/// Gets or sets the minimum length.
		/// </summary>
		/// <value>
		/// The minimum length.
		/// </value>
		public int MinimumLength { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryPasswordAttribute" /> class.
		/// </summary>
		public DictionaryPasswordAttribute()
			: this(Membership.MinRequiredPasswordLength, "Validation Field Password String Length") { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryPasswordAttribute" /> class.
		/// </summary>
		public DictionaryPasswordAttribute(string dictionaryKey)
			: this(Membership.MinRequiredPasswordLength, dictionaryKey) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryPasswordAttribute"/> class.
		/// </summary>
		/// <param name="minimumLength">The minimum length.</param>
		public DictionaryPasswordAttribute(int minimumLength)
			: this(minimumLength, "Validation Field Password String Length") { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryPasswordAttribute"/> class.
		/// </summary>
		/// <param name="minimumLength">The minimum length.</param>
		/// <param name="dictionaryKey">The dictionary key.</param>
		public DictionaryPasswordAttribute(int minimumLength, string dictionaryKey)
		{
			this.MinimumLength = minimumLength;
			_dictionaryKey = dictionaryKey;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (this.MinimumLength > 0 && value == null || value.ToString().Length < this.MinimumLength)
			{
				return new ValidationResult(
					GetErrorMessage(validationContext.DisplayName),
					new[] { validationContext.MemberName });
			}

			return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			ModelClientValidationRule rule = new ModelClientValidationRule
			{
				ErrorMessage = GetErrorMessage(metadata.GetDisplayName()),
				ValidationType = "length",
			};

			rule.ValidationParameters.Add("min", this.MinimumLength);

			yield return rule;
		}

		private string GetErrorMessage(string displayName)
		{
			this.ErrorMessage = DictionaryHelper.GetDictionaryValue(key: _dictionaryKey);

			return string.Format(this.ErrorMessage, displayName, this.MinimumLength);
		}

	}
}
