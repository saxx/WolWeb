using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WolWeb {
    public class AuthorizeRemoteOnlyAttribute : AuthorizeAttribute {

        public override void OnAuthorization(AuthorizationContext filterContext) {
            if (filterContext.HttpContext.Request.IsLocal)
                filterContext.Result = null;
            else
                base.OnAuthorization(filterContext);
        }

    }
}