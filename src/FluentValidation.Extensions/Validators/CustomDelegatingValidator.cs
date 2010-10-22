namespace FluentValidation.Extensions.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Resources;
    using Results;
    using FluentValidation.Validators;
    using System.Linq.Expressions;

    public class CustomDelegatingValidator : DelegatingValidator
    {
        public readonly Expression Expression;
        public CustomDelegatingValidator(Predicate<object> condition, IPropertyValidator innerValidator, Expression expression)
            : base(condition, innerValidator)
        {
            this.Expression = expression;
        }
    }
}