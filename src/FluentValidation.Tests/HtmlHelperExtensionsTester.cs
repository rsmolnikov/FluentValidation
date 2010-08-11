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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using Results;
    using System.Web.Mvc;
    using FluentValidation.Mvc;
    using System.IO;
    using FluentValidation.Attributes;
    using System.Web.Mvc.Html;
    using System.Web;

	[TestFixture]
    public class HtmlHelperExtensionsTester
    {
        private HtmlHelper htmlHelper;
        private FluentValidationModelValidatorProvider provider;
        private const string defaultJsonOutputForModel = "\r\n<script type=\"text/javascript\">\r\nif (clientValidation == undefined) var clientValidation = new Array(); clientValidation[\"form_1\"] =  {\"ns\":\"form_1\",\"rules\":[{\"Field\":\"UserName\",\"Attributes\":[{\"ErrorMessage\":\"UserName field must be from 2 to 20 length\",\"ValidationParameters\":{\"minimumLength\":2,\"maximumLength\":20},\"ValidationType\":\"stringLength\"}]},{\"Field\":\"Email\",\"Attributes\":[{\"ErrorMessage\":\"Not valid email\",\"ValidationParameters\":{\"typeName\":\"email\"},\"ValidationType\":\"type\"}]},{\"Field\":\"Age\",\"Attributes\":[{\"ErrorMessage\":\"Digits only\",\"ValidationParameters\":{\"typeName\":\"digits\"},\"ValidationType\":\"type\"}]}]};\r\n</script>";
       
        [TestFixtureSetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
            ModelValidatorProviders.Providers.Add(provider);
            ViewContext context = new ViewContext();
            context.Writer = new StringWriter();
            IViewDataContainer container = new ViewDataContainer();
            container.ViewData = new ViewDataDictionary(new Model());
            htmlHelper = new System.Web.Mvc.HtmlHelper(context, container);
		}

        [Test]
        public void GetClientValidationJson_Test()
        {
            
            HtmlHelperExtensions.GetClientValidationJson(htmlHelper, "form_1");
            htmlHelper.ViewContext.Writer.ToString().ShouldEqual(defaultJsonOutputForModel);
           
        }

        [TestFixtureTearDown]
        public void Clear()
        {
            htmlHelper = null;
            ModelValidatorProviders.Providers.Remove(provider);
        }

        private class ViewDataContainer : IViewDataContainer
        {
            #region IViewDataContainer Members

            public ViewDataDictionary ViewData { get; set; }

            #endregion
        }
        [Validator(typeof(Validator))]
        private class Model
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Age { get; set; }

            public class Validator : AbstractValidator<Model>
            {
                public Validator()
                {
                    RuleFor(x => x.UserName).Length(2, 20).WithMessage("UserName field must be from 2 to 20 length");
                    RuleFor(x => x.Email).IsOfType(Validators.ValidatableTypes.email).WithMessage("Not valid email");
                    RuleFor(x => x.Age).IsOfType(Validators.ValidatableTypes.digits).WithMessage("Digits only");
                }
            }
        }
	}
}