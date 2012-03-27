using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using vancouveropendata.Controllers;
using DataEnums = ODAF.Data.Enums;
using System.Net;
using System.Configuration;

namespace vancouveropendata
{
    /// <summary>
    /// Specifies that various preconditions must exist before the action is executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PrerequisiteAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Indicates whether the user has needs this access level before the action is executed.
        /// </summary>
        public DataEnums.UserRole Role { set; get; }

        /// <summary>
        /// If true, user is shown a view, otherwise a Json result is returned.
        /// </summary>
        public bool ShowPage { set; get; }

        /// <summary>
        /// Indicates whether the user must be authenticated before the action is executed.
        /// </summary>
        public bool Authenticated { set; get; }

        #region IAuthorizationFilter Members

        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            bool devMode = false;
            bool.TryParse(ConfigurationManager.AppSettings["DevMode"], out devMode);
            if (devMode)
            {
                return;
            }

            BaseController controller = filterContext.Controller as BaseController;
            bool isLoggedIn = controller.IsLoggedIn();

            if (Authenticated)
            {
                if (!isLoggedIn)
                {
                    if (!ShowPage)
                    {
                        filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        JsonResult result = new JsonResult();
                        result.Data = new { };
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = result;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect("~/admin/accessdenied");
                    }
                }
                else // Checking for Roles only applies to logged in users
                {
                    if ((DataEnums.UserRole)controller.AuthenticatedUser.UserRole < Role)
                    {
                        if (!ShowPage)
                        {
                            filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            JsonResult result = new JsonResult();
                            result.Data = new { error = "User does not have appropriate Role." };
                            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                            filterContext.Result = result;
                        }
                        else
                        {
                            filterContext.HttpContext.Response.Redirect("~/admin/accessdeniedtorole");
                        }
                    }
                }
            }
        }

        #endregion
    }
}
