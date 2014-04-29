using Souls.Client.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SoulsClient.Filters
{
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                                             typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            /*if (AuthorizeCore(filterContext.HttpContext))
            {

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null );
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }*/


            if (cSession.Current != null)
            {
                // JUST to be sure, check that userID is not -1 (not valid)
                if (cSession.Current.player == null)
                {
                    filterContext.Result = RedirectToLogin();
                }

                //////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////
                // Maybe do aditional checks for every pageload etc?
                // like is it possible to create a Fake Session?
                



            }
            else // Auth is fail.
            {
                filterContext.Result = RedirectToLogin();//new HttpStatusCodeResult(401);
            }
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public ActionResult RedirectToLogin()
        {
            return new RedirectToRouteResult(
                                   new RouteValueDictionary 
                                   {
                                       { "action", "Login" },
                                       { "controller", "Player" }
                                   });
        }
    }
}
