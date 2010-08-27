namespace FluentValidation.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
    using FluentValidation;
    using FluentValidation.Results;

	/// <summary>
	/// ModelValidator implementation that uses FluentValidation.
	/// </summary>
	internal class CustomFluentValidationModelValidator : ModelValidator {
		readonly IValidator validator;

        public CustomFluentValidationModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidator validator)
			: base(metadata, controllerContext) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (Metadata.Model != null) {

				var result = validator.Validate(Metadata.Model);

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult {
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}
	}
}