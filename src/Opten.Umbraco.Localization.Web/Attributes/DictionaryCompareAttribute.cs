using Opten.Umbraco.Localization.Web.Extensions;
using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// Compares two Fields. The Error Message is resolved from the Umbraco's Dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DictionaryCompareAttribute : ValidationAttribute, IClientValidatable
	{

		private readonly string _dictionaryKey;

		/// <summary>
		/// Gets or sets the other property.
		/// </summary>
		/// <value>
		/// The other property.
		/// </value>
		public string OtherProperty { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryCompareAttribute"/> class.
		/// </summary>
		/// <param name="otherProperty">The other property.</param>
		public DictionaryCompareAttribute(string otherProperty)
			: this(otherProperty, "Validation Field Compare")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryCompareAttribute"/> class.
		/// </summary>
		/// <param name="otherProperty">The other property.</param>
		/// <param name="dictionaryKey">The dictionary key.</param>
		public DictionaryCompareAttribute(string otherProperty, string dictionaryKey)
		{
			this.OtherProperty = otherProperty;
			_dictionaryKey = dictionaryKey;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			Type containerType = validationContext.ObjectInstance.GetType();
			PropertyInfo otherProperty = containerType.GetProperty(this.OtherProperty);

			//TODO: Throw error if OtherProperty not found?
			if (otherProperty != null)
			{
				object otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

				string otherValue = (otherPropertyValue == null) ? string.Empty : otherPropertyValue.ToString();
				string currentValue = (value == null) ? string.Empty : value.ToString();

				if (Equals(otherValue, currentValue) == false)
				{
					return new ValidationResult(
						GetErrorMessage(validationContext.DisplayName, otherProperty),
						new[] { validationContext.MemberName });
				}
			}

			return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			PropertyInfo otherProperty = metadata.ContainerType.GetProperty(this.OtherProperty);

			if (otherProperty == null)
			{
				throw new NullReferenceException(string.Format("Property '{0}' in your class not found. Maybe you spelled something wrong?", this.OtherProperty));
			}

			ModelClientValidationRule rule = new ModelClientValidationRule
			{
				ErrorMessage = GetErrorMessage(metadata.GetDisplayName(), otherProperty),
				ValidationType = "equalto",
			};

			rule.ValidationParameters.Add("other", "*." + this.OtherProperty);

			yield return rule;
		}

		private string GetErrorMessage(string displayName, PropertyInfo otherProperty)
		{
			//TODO: We can't check if it's null or white space, because when we change the language it won't be set
			// is there a way that we can check if the language changed?

			//if(string.IsNullOrWhiteSpace(this.ErrorMessage))
			//{
			this.ErrorMessage = DictionaryHelper.GetDictionaryValue(key: _dictionaryKey);
			//}

			string displayName2 = otherProperty.GetDisplayName();

			return string.Format(this.ErrorMessage, displayName2, displayName);
		}

	}
}
