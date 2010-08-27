namespace FluentValidation.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Validators;
	using System.Linq;
    using FluentValidation.Mvc;
    using FluentValidation.Validators;
    using System.Reflection;

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