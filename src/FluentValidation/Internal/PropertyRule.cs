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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Results;
	using Validators;

	public class PropertyRule<T> : IValidationRule<T> {
		readonly List<IPropertyValidator> validators = new List<IPropertyValidator>();
		Func<CascadeMode> cascadeModeThunk = () => ValidatorOptions.CascadeMode;

		public MemberInfo Member { get; private set; }
		public PropertySelector PropertyFunc { get; private set; }
		public Expression Expression { get; private set; }
		public string CustomPropertyName { get; set; }
		public Action<object> OnFailure { get; set; }
		public IPropertyValidator CurrentValidator { get; private set; }
		public Type TypeToValidate { get; private set; }

		public CascadeMode CascadeMode {
			get { return cascadeModeThunk(); }
			set { cascadeModeThunk = () => value; }
		}

		public IEnumerable<IPropertyValidator> Validators {
			get { return validators.AsReadOnly(); }
		}

		public PropertyRule(MemberInfo member, PropertySelector propertyFunc, Expression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate) {
			Member = member;
			PropertyFunc = propertyFunc;
			Expression = expression;
			OnFailure = x => { };
			TypeToValidate = typeToValidate;
			this.cascadeModeThunk = cascadeModeThunk;

			PropertyName = ValidatorOptions.PropertyNameResolver(typeof(T), member);
		}

		public static PropertyRule<T> Create<TProperty>(Expression<Func<T, TProperty>> expression) {
			return Create(expression, () => ValidatorOptions.CascadeMode);
		}

		public static PropertyRule<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk) {
			var member = expression.GetMember();
			var compiled = expression.Compile();
			PropertySelector propertySelector = x => compiled((T)x);

			return new PropertyRule<T>(member, propertySelector, expression, cascadeModeThunk, typeof(TProperty));
		}

		public void AddValidator(IPropertyValidator validator) {
			CurrentValidator = validator;
			validators.Add(validator);
		}

		public void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator) {
			var index = validators.IndexOf(original);
			
			if(index > -1) {
				validators[index] = newValidator;

				if(ReferenceEquals(CurrentValidator, original)) {
					CurrentValidator = newValidator;
				}
			}
		}

		[Obsolete("Use ReplaceValiadtor instead.")]
		public void ReplaceCurrentValidtor(IPropertyValidator newValidator) {
			ReplaceValidator(CurrentValidator, newValidator);
		}

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName { get; set; }

		public string PropertyDescription {
			get { return CustomPropertyName ?? PropertyName.SplitPascalCase(); }
		}

		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			var cascade = cascadeModeThunk();
			bool hasAnyFailure = false;

			foreach (var validator in validators) {
				var results = InvokePropertyValidator(context, validator);

				bool hasFailure = false;

				foreach (var result in results) {
					hasAnyFailure = true;
					hasFailure = true;
					yield return result;
				}

				if (cascade == FluentValidation.CascadeMode.StopOnFirstFailure && hasFailure) {
					break;
				}
			}

			if (hasAnyFailure) {
				OnFailure(context.InstanceToValidate);
			}
		}

		protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(ValidationContext<T> context, IPropertyValidator validator) {
			if (PropertyName == null && CustomPropertyName == null) {
				throw new InvalidOperationException(string.Format("Property name could not be automatically determined for expression {0}. Please specify either a custom property name by calling 'WithName'.", Expression));
			}

			string propertyName = BuildPropertyName(context);

			if (context.Selector.CanExecute(this, propertyName)) {
				var validationContext = new PropertyValidatorContext(PropertyDescription, context.InstanceToValidate, x => PropertyFunc((T)x), propertyName, Member);
				validationContext.PropertyChain = context.PropertyChain;
				return validator.Validate(validationContext);
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		private string BuildPropertyName(ValidationContext<T> context) {
			return context.PropertyChain.BuildPropertyName(PropertyName ?? CustomPropertyName);
		}
	}
}