#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Results;
	using Validators;

	/// <summary>
	/// Builds a validation rule and constructs a validator.
	/// </summary>
	/// <typeparam name="T">Type of object being validated</typeparam>
	/// <typeparam name="TProperty">Type of property being validated</typeparam>
	public class RuleBuilder<T, TProperty> : IRuleBuilderOptions<T, TProperty>, IRuleBuilderInitial<T, TProperty> {
		readonly PropertyRule<T> rule;
		Func<CascadeMode> cascadeMode = () => ValidatorOptions.CascadeMode;

		public CascadeMode CascadeMode {
			get { return cascadeMode(); }
			set { cascadeMode = () => value; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RuleBuilder{T,TProperty}">RuleBuilder</see> class.
		/// </summary>
		/// <param name="expression">Property expression used to initialise the rule builder.</param>
		/// <param name="rule">Underlying property rule</param>
		public RuleBuilder(PropertyRule<T> rule) {
			this.rule = rule;
		}

		/// <summary>
		/// Sets the validator associated with the rule.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		/// <returns></returns>
		public IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator) {
			validator.Guard("Cannot pass a null validator to SetValidator.");
			rule.AddValidator(validator);
			return this;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator) {
			validator.Guard("Cannot pass a null validator to SetValidator");
			SetValidator(new ChildValidatorAdaptor<TProperty>(validator));
			return this;
		}

		public IRuleBuilderOptions<T, TProperty> Configure(Action<PropertyRule<T>> configurator) {
			configurator(rule);
			return this;
		}

		public IRuleBuilderInitial<T, TProperty> Configure(Action<RuleBuilder<T, TProperty>> configurator) {
			configurator(this);
			return this;
		}
	}
}