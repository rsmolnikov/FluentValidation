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
	using System.Collections;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class TypeValidatorTests {

		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_type_are_matched_digits_type_validation_should_succeed() 
        {
            ValidateByTypeValidator("1221",ValidatableTypes.digits).ShouldBeTrue();
		}

		[Test]
        public void When_the_type_are_not_matched_digits_type_validation_should_fail()
        {
            ValidateByTypeValidator("23.45", ValidatableTypes.digits).ShouldBeFalse();
		}

        [Test]
        public void When_the_type_are_matched_date_type_validation_should_succeed()
        {
            ValidateByTypeValidator("12.12.2010", ValidatableTypes.date).ShouldBeTrue();
        }

        [Test]
        public void When_the_type_are_not_matched_date_type_validation_should_fail()
        {
            ValidateByTypeValidator("231sdsad", ValidatableTypes.date).ShouldBeFalse();
        }

        [Test]
        public void When_the_type_are_matched_dateISO_type_validation_should_succeed()
        {
            ValidateByTypeValidator("2010-12-12", ValidatableTypes.dateISO).ShouldBeTrue();
        }

        [Test]
        public void When_the_type_are_not_matched_dateISO_type_validation_should_fail()
        {
            ValidateByTypeValidator("3454fdsf", ValidatableTypes.dateISO).ShouldBeFalse();
        }

        [Test]
        public void When_the_type_are_matched_email_type_validation_should_succeed()
        {
            ValidateByTypeValidator("bender@planetexpress.us", ValidatableTypes.email).ShouldBeTrue();
        }

        [Test]
        public void When_the_type_are_not_matched_email_type_validation_should_fail()
        {
            ValidateByTypeValidator("hawaii.com", ValidatableTypes.email).ShouldBeFalse();
        }

        [Test]
        public void When_the_type_are_matched_number_type_validation_should_succeed()
        {
            ValidateByTypeValidator("3.1415926535", ValidatableTypes.number).ShouldBeTrue();
        }

        [Test]
        public void When_the_type_are_not_matched_number_type_validation_should_fail()
        {
            ValidateByTypeValidator("pi", ValidatableTypes.number).ShouldBeFalse();
        }

        [Test]
        public void When_the_type_are_matched_url_type_validation_should_succeed()
        {
            ValidateByTypeValidator("http://stalin.su", ValidatableTypes.url).ShouldBeTrue();
        }

        [Test]
        public void When_the_type_are_not_matched_url_type_validation_should_fail()
        {
            ValidateByTypeValidator("hitler@", ValidatableTypes.url).ShouldBeFalse();
        }

        [Test]
        public void When_the_validator_fails_the_error_message_should_be_set()
        {
            var validator = new TypeValidator(ValidatableTypes.url);
            var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => "sdsds"));
            result.Single().ErrorMessage.ShouldEqual("The value must be of type 'url'");
        }

        private bool ValidateByTypeValidator(string target, ValidatableTypes type)
        {
            var validator = new TypeValidator(type);
            var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => target));
            return result.IsValid();
        }

        //[Test]
        //public void When_validation_fails_the_error_should_be_set() {
        //    var person = new Person() {Forename = "Bar"};
        //    var validator = CreateValidator(x => x.Forename);
        //    var result = validator.Validate(new PropertyValidatorContext("Forename", person, x => "Foo"));
        //    result.Single().ErrorMessage.ShouldEqual("'Forename' should be equal to 'Bar'.");
        //}

        //[Test]
        //public void Should_store_property_to_compare() {
        //    var validator = CreateValidator(x => x.Surname);
        //    validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
        //}

        //[Test]
        //public void Should_store_comparison_type() {
        //    var validator = CreateValidator(x => x.Surname);
        //    validator.Comparison.ShouldEqual(Comparison.Equal);
        //}

        //[Test]
        //public void Validates_against_constant() {
        //    var validator = new EqualValidator("foo");
        //    var result = validator.Validate(new PropertyValidatorContext(null, new Person(), x => "bar"));
        //    result.IsValid().ShouldBeFalse();
        //}

        //[Test]
        //public void Should_succeed_on_case_insensitive_comparison() {
        //    var person = new Person { Surname = "foo" };
        //    var validator = CreateValidator(x => x.Surname, StringComparer.OrdinalIgnoreCase);
        //    var context = new PropertyValidatorContext("Surname", person, x => "FOO");

        //    var result = validator.Validate(context);
        //    result.IsValid().ShouldBeTrue();
        //}

       
	}
}