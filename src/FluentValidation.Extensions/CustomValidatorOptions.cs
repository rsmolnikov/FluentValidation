namespace FluentValidation.Extensions
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Resources;
	using Validators;
    using System.Reflection.Emit;

	public static class CustomValidatorOptions {

		/// <summary>
		/// Specifies a condition limiting when the validator should run. 
		/// The validator will only be executed if the result of the lambda returns true.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
		/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
		/// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> ExWhen<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<T, bool>> expression, ApplyConditionTo applyConditionTo)
        {
            Func<T, bool> predicate = expression.Compile();
            if (predicate == null) throw new ArgumentNullException("A predicate must be specified when calling When.");
            if (expression.Parameters.Count > 1) throw new ArgumentNullException("A predicate with multiple parameters not supported. (See Client-Side Validation.)");
			return rule.Configure(config => {
				// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
				if (applyConditionTo == ApplyConditionTo.AllValidators) {
					foreach (var validator in config.Validators.ToList()) {
						var wrappedValidator = new CustomDelegatingValidator(x => predicate((T)x), validator, expression.Body);
						config.ReplaceValidator(validator, wrappedValidator);
					}
				}
				else {
                    var wrappedValidator = new CustomDelegatingValidator(x => predicate((T)x), config.CurrentValidator, expression.Body);
					config.ReplaceValidator(config.CurrentValidator, wrappedValidator);
				}
			});
		}

		
		/// <summary>
		/// Specifies a condition limiting when the validator should run. 
		/// The validator will only be executed if the result of the lambda returns true.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
		/// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> ExWhen<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<T, bool>> expression)
        {
            return rule.ExWhen(expression, ApplyConditionTo.AllValidators);
		}
	}
}