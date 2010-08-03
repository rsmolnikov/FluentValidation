namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Validators;
	using System.Linq;

	public class FluentValidationPropertyValidator : ModelValidator {
		protected readonly IPropertyValidator validator;

		/*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		*/
		protected bool ShouldValidate { get; set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var context = new PropertyValidatorContext(Metadata.PropertyName, container, Metadata.Model, Metadata.PropertyName);
				var result = validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

		public static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new FluentValidationPropertyValidator(meta, context, validator);
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}
	}

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {
			bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

			ShouldValidate = isNonNullableValueType && nullWasSpecified;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new RequiredFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationRequiredRule(validator.ErrorMessageSource.BuildErrorMessage()) };
		}

		public override bool IsRequired {
			get { return true; }
		}
	}

	internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)validator; }
		}

		public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {
			ShouldValidate = false;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new StringLengthFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationStringLengthRule(LengthValidator.ErrorMessageSource.BuildErrorMessage(), LengthValidator.Min, LengthValidator.Max) };
		}
	}

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)validator;}
		}

		public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {
			ShouldValidate = false;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new RegularExpressionFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationRegexRule(RegexValidator.ErrorMessageSource.BuildErrorMessage(), RegexValidator.Expression) };

		}
	}

    internal class TypeFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        ITypeValidator TypeValidator
        {
            get { return (ITypeValidator)validator; }
        }

        public TypeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new TypeFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationType(TypeValidator.ErrorMessageSource.BuildErrorMessage(), TypeValidator.TypeName) };
        }


    }
    
    internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        IBetweenValidator RangeValidator
        {
            get { return (IBetweenValidator)validator; }
        }

        public RangeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new RangeFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            bool exclusive = RangeValidator.GetType().Equals(typeof(ExclusiveBetweenValidator));
            return new[] { new ModelClientValidationRange(RangeValidator.ErrorMessageSource.BuildErrorMessage(), RangeValidator.From, RangeValidator.To, exclusive) };

        }
    }
    
    internal class EqualFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        IComparisonValidator EqualValidator
        {
            get { return (IComparisonValidator)validator; }
        }

        public EqualFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new EqualFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationEqual(EqualValidator.ErrorMessageSource.BuildErrorMessage(), EqualValidator.MemberToCompare) };

        }
    }
    
}