using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace csharp_web_exam.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Verificar si existe el token en la cookie
            string token = httpContext.Request.Cookies["AuthToken"]?.Value;
            string username = httpContext.Session["Username"] as string;

            return !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirigir a la página de login con returnUrl
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login",
                    returnUrl = filterContext.HttpContext.Request.RawUrl
                })
            );
        }
    }
}
