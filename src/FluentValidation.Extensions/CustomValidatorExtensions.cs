namespace FluentValidation.Extensions
{
    using Validators;
    using FluentValidation;

	/// <summary>
	/// Extension methods that provide the default set of validators.
	/// </summary>
	public static class CustomValidatorExtensions {

        /// <summary>
        /// Defines a type validator on the current rule builder.
        /// Validation will fail if the value returned by the lambda is not a matches specified type.
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="typeName">The name of the type to compare.</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> IsOfType<T>(this IRuleBuilder<T, string> ruleBuilder, ValidatableTypes typeName)
        {
            return ruleBuilder.SetValidator(new TypeValidator(typeName));
        }
	}
}