using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ClientSideValidation.Models;

namespace ClientSideValidation.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        protected override void Initialize(RequestContext requestContext)
        {
           

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

       

    }
}
