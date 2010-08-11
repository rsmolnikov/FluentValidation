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

namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Mvc;
	using NUnit.Framework;
	using FluentValidation.Mvc.MetadataExtensions;
    using System.Web.Mvc;

	[TestFixture]
	public class FluentValidationModelClientValidationRulesTester {

		[Test]
		public void GetModelClientValidationType_returns_typename_and_parameters() {
            ModelClientValidationRule rule = new ModelClientValidationType("Error msg", Validators.ValidatableTypes.digits);

            rule.ErrorMessage.ShouldNotBeNull();
            rule.ValidationType.ShouldBeTheSameAs("type");
            rule.ValidationParameters.ContainsKey("typeName").ShouldBeTrue();
            rule.ValidationParameters["typeName"].ShouldEqual(Convert.ToString(Validators.ValidatableTypes.digits));

		}
        [Test]
        public void GetModelClientValidationRange_returns_typename_and_parameters()
        {
            ModelClientValidationRule rule = new ModelClientValidationRange("Error msg",10,20,false);

            rule.ErrorMessage.ShouldNotBeNull();
            rule.ValidationType.ShouldBeTheSameAs("range");
            rule.ValidationParameters.ContainsKey("minimum").ShouldBeTrue();
            rule.ValidationParameters["minimum"].ShouldEqual(Convert.ToString(10));
            rule.ValidationParameters.ContainsKey("maximum").ShouldBeTrue();
            rule.ValidationParameters["maximum"].ShouldEqual(Convert.ToString(20));
            rule.ValidationParameters.ContainsKey("exclusive").ShouldBeTrue();
            rule.ValidationParameters["exclusive"].ShouldEqual(Convert.ToString(false).ToLower());
        }

        [Test]
        public void GetModelClientValidationEqual_returns_typename_and_parameters()
        {
            ModelClientValidationRule rule = new ModelClientValidationEqual("Error msg", new object().GetType());

            rule.ErrorMessage.ShouldNotBeNull();
            rule.ValidationType.ShouldBeTheSameAs("equalTo");
            rule.ValidationParameters.ContainsKey("equalTo").ShouldBeTrue();
            rule.ValidationParameters["equalTo"].ShouldEqual(Convert.ToString("Object"));
        }

       
	}
}