namespace FluentValidation.Extensions.Mvc
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Validators;
    using FluentValidation.Mvc;
    using FluentValidation.Validators;
using System.Linq.Expressions;

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

    internal class DelegatingFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        CustomDelegatingValidator DelegatingValidator
        {
            get { return (CustomDelegatingValidator)validator; }
        }
        readonly ModelValidator innerValidator;

        public DelegatingFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator) {
            innerValidator = ExtendedFluentValidationModelValidatorProvider.GetModelValidatorFromFullList(metadata, controllerContext, ((IDelegatingValidator)validator).InnerValidator);
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new DelegatingFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationDelegating(innerValidator.GetClientValidationRules().First(), DelegatingValidator.Expression) };

        }
    }

//yea, crappy internal classes....
#region Copy_Of_PropepertyValidators_From_FluentValidation.Mvc
    internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
            bool nullWasSpecified = metadata.Model == null;

            ShouldValidate = isNonNullableValueType && nullWasSpecified;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new RequiredFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationRequiredRule(validator.ErrorMessageSource.BuildErrorMessage()) };
        }

        public override bool IsRequired
        {
            get { return true; }
        }
    }

    internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        private ILengthValidator LengthValidator
        {
            get { return (ILengthValidator)validator; }
        }

        public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new StringLengthFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationStringLengthRule(LengthValidator.ErrorMessageSource.BuildErrorMessage(), LengthValidator.Min, LengthValidator.Max) };
        }
    }

    internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        IRegularExpressionValidator RegexValidator
        {
            get { return (IRegularExpressionValidator)validator; }
        }

        public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator)
            : base(metadata, controllerContext, validator)
        {
            ShouldValidate = false;
        }

        public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator)
        {
            return new RegularExpressionFluentValidationPropertyValidator(meta, context, validator);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationRegexRule(RegexValidator.ErrorMessageSource.BuildErrorMessage(), RegexValidator.Expression) };

        }
    }
#endregion
}