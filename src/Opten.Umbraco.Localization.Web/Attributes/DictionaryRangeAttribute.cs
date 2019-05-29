using Opten.Umbraco.Localization.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Attributes
{
	/// <summary>
	/// DictionaryRangeAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DictionaryRangeAttribute : RangeAttribute, IClientValidatable
	{

		private readonly string _dictionaryKey;
		private readonly object _max;
		private readonly object _min;

		/// <summary>
		/// DictionaryRangeAttribute
		/// </summary>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="dictionaryKey"></param>
		public DictionaryRangeAttribute(int minimum, int maximum, string dictionaryKey) : base(minimum, maximum)
		{
			_dictionaryKey = dictionaryKey;
			_max = maximum;
			_min = minimum;
		}

		/// <summary>
		/// DictionaryRangeAttribute
		/// </summary>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="dictionaryKey"></param>
		public DictionaryRangeAttribute(double minimum, double maximum, string dictionaryKey) : base(minimum, maximum)
		{
			_dictionaryKey = dictionaryKey;
			_max = maximum;
			_min = minimum;
		}

		/// <summary>
		/// DictionaryRangeAttribute
		/// </summary>
		/// <param name="type"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="dictionaryKey"></param>
		public DictionaryRangeAttribute(Type type, string minimum, string maximum, string dictionaryKey) : base(type, minimum, maximum)
		{
			_dictionaryKey = dictionaryKey;
			_max = maximum;
			_min = minimum;
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

			var rule = new ModelClientValidationRule
			{
				ErrorMessage = GetErrorMessage(displayName),
				ValidationType = "range",
			};

			rule.ValidationParameters.Add("min", _min);
			rule.ValidationParameters.Add("max", _max);

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
