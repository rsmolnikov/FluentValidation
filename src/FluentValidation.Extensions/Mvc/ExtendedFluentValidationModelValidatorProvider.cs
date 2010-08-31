namespace FluentValidation.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using Validators;
    using FluentValidation;
    using FluentValidation.Validators;
    using FluentValidation.Mvc;

    public class ExtendedFluentValidationModelValidatorProvider : FluentValidationModelValidatorProvider
    {
        readonly IValidatorFactory validatorFactory;

        private Dictionary<Type, FluentValidationModelValidationFactory> validatorFactories = new Dictionary<Type, FluentValidationModelValidationFactory>() {
            { typeof(ITypeValidator), TypeFluentValidationPropertyValidator.Create },
            { typeof(IBetweenValidator), RangeFluentValidationPropertyValidator.Create },
            { typeof(EqualValidator), EqualFluentValidationPropertyValidator.Create },
		};
       
        public ExtendedFluentValidationModelValidatorProvider(IValidatorFactory validatorFactory):base(validatorFactory)
        {
			AddImplicitRequiredValidator = true;
			this.validatorFactory = validatorFactory;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context) {
            var modelValidators = new List<ModelValidator>();
            modelValidators.AddRange(base.GetValidators(metadata,context));
            if (IsValidatingProperty(metadata)) {
				modelValidators.AddRange(GetValidatorsForProperty(metadata, context, validatorFactory.GetValidator(metadata.ContainerType)));
                return modelValidators;
			}

			modelValidators.AddRange(GetValidatorsForModel(metadata, context, validatorFactory.GetValidator(metadata.ModelType)));
            return modelValidators;
		}

		IEnumerable<ModelValidator> GetValidatorsForProperty(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			var modelValidators = new List<ModelValidator>();

			if (validator != null) {
				var descriptor = validator.CreateDescriptor();

                var validators = descriptor.GetValidatorsForMember(metadata.PropertyName);
                    //Equal validator with MemberInfo not support standalone
					//.Where(x => x.SupportsStandaloneValidation);

				foreach(var propertyValidator in validators) {
					modelValidators.Add(GetModelValidator(metadata, context, propertyValidator));
				}
			}

			return modelValidators;
		}

		private ModelValidator GetModelValidator(ModelMetadata meta, ControllerContext context, IPropertyValidator propertyValidator) {
			var type = propertyValidator.GetType();
			
			var factory = validatorFactories
				.Where(x => x.Key.IsAssignableFrom(type))
				.Select(x => x.Value)
				.FirstOrDefault() ?? FluentValidationPropertyValidator.Create;

			return factory(meta, context, propertyValidator);
		}

		IEnumerable<ModelValidator> GetValidatorsForModel(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			if (validator != null) {
                yield return new CustomFluentValidationModelValidator(metadata, context, validator);
			}
		}

		bool IsValidatingProperty(ModelMetadata metadata) {
			return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
		}
	}
}