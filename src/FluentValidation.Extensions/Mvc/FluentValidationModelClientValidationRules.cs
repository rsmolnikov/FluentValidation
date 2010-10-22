namespace FluentValidation.Extensions.Mvc
{
    using System;
    using System.Web.Mvc;
    using System.Reflection;
    using Validators;
using System.Linq.Expressions;
    using FluentValidation.Extensions.Utilities;

    /// <summary>
    /// ModelClientValidationRule implementations
    /// </summary>
    public class ModelClientValidationType : ModelClientValidationRule
    {
        public ModelClientValidationType(string errorMessage, ValidatableTypes typeName)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "type";
            base.ValidationParameters.Add("typeName", Convert.ToString(typeName));
        }
    }

    public class ModelClientValidationRange : ModelClientValidationRule
    {
        public ModelClientValidationRange(string errorMessage, IComparable from, IComparable to, bool exclusive)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "range";
            base.ValidationParameters.Add("minimum", Convert.ToString(from));
            base.ValidationParameters.Add("maximum", Convert.ToString(to));
            base.ValidationParameters.Add("exclusive", Convert.ToString(exclusive).ToLower());
        }
    }

    public class ModelClientValidationEqual : ModelClientValidationRule
    {
        public ModelClientValidationEqual(string errorMessage, MemberInfo memberToCompare)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "equalTo";
            base.ValidationParameters.Add("equalTo", Convert.ToString(memberToCompare.Name));
        }
    }

    public class ModelClientValidationDelegating : ModelClientValidationRule
    {
        public ModelClientValidationDelegating(ModelClientValidationRule innerValidatoinRule, Expression expression)
        {
            base.ErrorMessage = innerValidatoinRule.ErrorMessage;
            base.ValidationType = "wrappedRule";
            base.ValidationParameters.Add("ruleType", Convert.ToString(innerValidatoinRule.ValidationType));
            base.ValidationParameters.Add("ruleParams", innerValidatoinRule.ValidationParameters );
            base.ValidationParameters.Add("expression", ExpressionParser.ConvertToJSCompliantString(expression));
        }
    }
}
