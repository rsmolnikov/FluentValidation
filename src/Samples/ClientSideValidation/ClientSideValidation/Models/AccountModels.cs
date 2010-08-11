using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Mvc;

namespace ClientSideValidation.Models
{
    [Validator(typeof(LogOnValidator))]
    public class LogOnModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
    }

    public class LogOnValidator : AbstractValidator<LogOnModel>
    {
        public LogOnValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().Length(2, 20).WithMessage("UserName field must be from 2 to 20 length");
            RuleFor(x => x.Email).NotEmpty().IsOfType(FluentValidation.Validators.ValidatableTypes.email).WithMessage("Not valid email");
            RuleFor(x => x.Age).NotEmpty().InclusiveBetween("10","20").WithMessage("Must be in range [10,20]").IsOfType(FluentValidation.Validators.ValidatableTypes.digits).WithMessage("Digits only");
            RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("Password not matched");
            RuleFor(x => x.Password).NotEmpty();
            
        }
    }
}
