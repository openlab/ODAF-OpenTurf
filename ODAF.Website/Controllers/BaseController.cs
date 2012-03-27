using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using ODAF.Data;

namespace vancouveropendata.Controllers
{
    public class BaseController : Controller
    {
        protected const string kTwitterUserId = "id";
        protected const string kTwitterScreenName = "screen_name";
        protected const string kTwitterProfileImageUrl = "profile_image_url";
        protected const string kAccountId = "account_id";

        public bool IsLoggedIn()
        {
            return (CurrentUserId != 0);
        }

        public string CurrentScreenName
        {
            get
            {
                return HttpContext.Session[kTwitterScreenName] as string;
            }
        }

        public long CurrentUserId
        {
            get
            {
                object o = HttpContext.Session[kAccountId];
                return o == null? 0 : (long)o;
            }
        }

        public OAuthAccount AuthenticatedUser
        {
            get
            {
                if (!_CachedAuthenticatedUser)
                {
                    _AuthenticatedUser = null;
                    if (IsLoggedIn() && (CurrentUserId != 0))
                    {
                        _AuthenticatedUser = OAuthAccount.Find(c => c.Id == CurrentUserId).SingleOrDefault();
                    }
                    else
                    {
                        _AuthenticatedUser = null;
                    }
                    _CachedAuthenticatedUser = true;
                }
                return _AuthenticatedUser;
            }
        }
        private OAuthAccount _AuthenticatedUser;
        private bool _CachedAuthenticatedUser;

        protected object CreateErrorObject(Exception ex)
        {
            // TODO: might not want to return StackTrace, but Message,
            // depending on DevMode
            return new { error = ex.StackTrace };
        }

    }
}
