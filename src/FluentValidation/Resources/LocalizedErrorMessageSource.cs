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

namespace FluentValidation.Resources {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	/// <summary>
	/// Implementation of IErrorMessageSource that uses a resource provider and resource name.
	/// </summary>
	public class LocalizedErrorMessageSource : IErrorMessageSource {
		static readonly Type defaultResourceType = typeof(Messages);
		readonly Func<string> accessor;
		readonly Type resourceType;
		readonly string resourceName;

		/// <summary>
		/// Creates a new instance of the LocalizedErrorMessageSource class using the specified resource name and resource type.
		/// </summary>
		/// <param name="resourceType">The resource type</param>
		/// <param name="resourceName">The resource name</param>
		public LocalizedErrorMessageSource(Type resourceType, string resourceName) {
			this.resourceType = resourceType;
			this.resourceName = resourceName;

			PropertyInfo property = null;

			if (resourceType == defaultResourceType && ValidatorOptions.ResourceProviderType != null) {
				property = ValidatorOptions.ResourceProviderType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);

				if (property != null) {
					resourceType = ValidatorOptions.ResourceProviderType;
				}
			}

			if (property == null) {
				property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
			}

			if (property == null) {
				throw new InvalidOperationException(string.Format("Could not find a property named '{0}' on type '{1}'.", resourceName, resourceType));
			}

			if (property.PropertyType != typeof(string)) {
				throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' does not return a string", resourceName, resourceType));
			}

			accessor = () => (string)property.GetValue(null, null);
		}

		/// <summary>
		/// Creates an IErrorMessageSource from an expression: () => MyResources.SomeResourceName
		/// </summary>
		/// <param name="expression">The expression </param>
		/// <returns>Error message source</returns>
		public static IErrorMessageSource CreateFromExpression(Expression<Func<string>> expression) {
			var constant = expression.Body as ConstantExpression;

			if (constant != null) {
				return new StringErrorMessageSource((string)constant.Value);
			}

			var member = expression.GetMember();

			if (member == null) {
				throw new InvalidOperationException("Only MemberExpressions an be passed to BuildResourceAccessor, eg () => Messages.MyResource");
			}

			var resourceType = member.DeclaringType;
			var resourceName = member.Name;
			return new LocalizedErrorMessageSource(resourceType, resourceName);
		}

		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		public string BuildErrorMessage() {
			return accessor();
		}

		/// <summary>
		/// The name of the resource if localized.
		/// </summary>
		public string ResourceName {
			get { return resourceName; }
		}

		/// <summary>
		/// The type of the resource provider if localized.
		/// </summary>
		public Type ResourceType {
			get { return resourceType; }
		}
	}
}