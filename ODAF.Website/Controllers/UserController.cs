using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using ODAF.Data;
using DataEnums = ODAF.Data.Enums;
using System.Configuration;
using OAuthLibrary;
using System.Transactions;
using SubSonic.DataProviders;
using System.Diagnostics;
using System.Net;
using Wrox.Twitter.NUrl;
using System.Web.Script.Serialization;
using System.Web.Security;
using website_mvc.Code;

namespace vancouveropendata.Controllers
{
    public class UserController : BaseController
    {
        private const string TwitterOAuthAuthorizeUrl = "https://api.twitter.com/oauth/authorize?oauth_token={0}";
        private const string TwitterVerifyCredentialsUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
        private const string TwitterUpdateStatusUrl = "https://api.twitter.com/1.1/statuses/update.json";

        private const string kOAuthToken = "oauth_token";
        private const string kOAuthTokenSecret = "oauth_token_secret";
        private const string kOAuthLink = "link";

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult RequestAuthToken(string appId)
        {
            object retVal = new { };

            OAuthClientApp app = OAuthClientApp.Find(c => c.Guid.Equals(appId)).SingleOrDefault();
            if (app == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Invalid or unknown appId" }, JsonRequestBehavior.AllowGet);
            }

            string call_return = OAuth.GetRequestToken(app.ConsumerKey, app.ConsumerSecret, app.CallbackUrl);

            var collection = HttpUtility.ParseQueryString(call_return);
            string token = collection[kOAuthToken];
            string token_secret = collection[kOAuthTokenSecret];

            if (token == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                retVal = new { error = call_return };
            }
            else
            {
                retVal = new
                {
                    oauth_token = token,
                    oauth_token_secret = token_secret,
                    link = String.Format(TwitterOAuthAuthorizeUrl, token)
                };
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAccessToken(string appId, string oauth_token, string oauth_verifier)
        {
            object retVal = new { };

            OAuthClientApp app = OAuthClientApp.Find(c => c.Guid.Equals(appId)).SingleOrDefault();
            if (app == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Invalid or unknown appId" }, JsonRequestBehavior.AllowGet);
            }

            string call_return = OAuth.GetAccessToken(app.ConsumerKey, app.ConsumerSecret, oauth_token, oauth_verifier);

            var collection = HttpUtility.ParseQueryString(call_return);
            string token = collection[kOAuthToken];
            string token_secret = collection[kOAuthTokenSecret];

            // if error, return
            if (token == null || token_secret == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = call_return }, JsonRequestBehavior.AllowGet);
            }

            DateTime? expiry = null;

            // reset the expiry token
            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    long tokenExpiryMinutes = 20;
                    long.TryParse(CloudSettingsResolver.GetConfigSetting("tokenExpiryMinutes"), out tokenExpiryMinutes);

                    OAuthAccount account = OAuthAccount.Find(c => c.oauth_token == token && c.oauth_service_id == app.Id).SingleOrDefault();
                    if (account != null)
                    {
                        account.TokenExpiry = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
                        account.Save();

                        ts.Complete();

                        expiry = account.TokenExpiry;
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(CreateErrorObject(ex), JsonRequestBehavior.AllowGet);
                }
            }

            // if the token expiry was reset, return it
            if (expiry.HasValue)
            {
                retVal = new
                {
                    oauth_token = token,
                    oauth_token_secret = token_secret,
                    oauth_token_expiry = expiry.Value
                };
            }
            else
            {
                retVal = new
                {
                    oauth_token = token,
                    oauth_token_secret = token_secret
                };
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Current(string format)
        {
            OAuthAccount account = AuthenticatedUser;

            if (account == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(account, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Authenticate(string appId, string oauth_token, string oauth_token_secret)
        {
            OAuthClientApp app = OAuthClientApp.Find(c => c.Guid.Equals(appId)).SingleOrDefault();
            if (app == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Invalid or unknown appId" }, JsonRequestBehavior.AllowGet);
            }

            OAuthAccount account = null;
            bool tokenExpired = TokenExpired(oauth_token, out account);

            // Check for UserAccess
            if (account != null)
            {
                if ((DataEnums.UserAccess)account.UserAccess != DataEnums.UserAccess.Normal)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
            }

            // Check for token expiry
            if (tokenExpired)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Expired token." }, JsonRequestBehavior.AllowGet);
            }

            // verify credentials with Twitter
            string verify = OAuth.GetProtectedResource(
                "GET",
                TwitterVerifyCredentialsUrl,
                app.ConsumerKey,
                app.ConsumerSecret,
                oauth_token,
                oauth_token_secret);

            HttpContext.Response.StatusCode = (int)NUrl.LastResponseStatusCode.GetValueOrDefault();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> obj = serializer.DeserializeObject(verify) as Dictionary<string, object>;

            if (HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                // Update/Add TwitterAccount
                // reset the expiry token
                using (TransactionScope ts = new TransactionScope())
                using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
                {
                    try
                    {
                        if (account == null)
                        {
                            account = new OAuthAccount();
                            account.CreatedOn = DateTime.UtcNow;
                            long tokenExpiryMinutes = 20;
                            long.TryParse(CloudSettingsResolver.GetConfigSetting("tokenExpiryMinutes"), out tokenExpiryMinutes);
                            account.TokenExpiry = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
                        }

                        // the deserializer always boxes it to int (so far), but just in case in the future its > int
                        long user_id = 0;

                        if (obj.ContainsKey(kTwitterUserId))
                        {
                            if (obj[kTwitterUserId] is int)
                            {
                                int user_id_int = (int)obj[kTwitterUserId];
                                user_id = user_id_int;
                            }
                            else if (obj[kTwitterUserId] is long)
                            {
                                user_id = (long)obj[kTwitterUserId];
                            }
                        }
                        string screen_name = obj[kTwitterScreenName] as string;
                        account.user_id = user_id;
                        account.screen_name = screen_name;
                        account.LastAccessedOn = DateTime.UtcNow;
                        account.oauth_token = oauth_token;
                        account.oauth_token_secret = oauth_token_secret;
                        account.oauth_service_id = app.Id;
                        account.profile_image_url = obj[kTwitterProfileImageUrl] as string;

                        var atu = CloudSettingsResolver.GetConfigSetting("AdminTwitterUser");

                        if (!string.IsNullOrEmpty(atu) && atu == screen_name)
                            account.UserRole = 2;

                        account.Save();

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(CreateErrorObject(ex), JsonRequestBehavior.AllowGet);
                    }
                }

                // Authenticate Session
                HttpContext.Session.RemoveAll();

                FormsAuthentication.SetAuthCookie(account.user_id.ToString(), false);
                HttpContext.Session[kAccountId] = account.Id;
                HttpContext.Session[kTwitterScreenName] = account.screen_name;
                HttpContext.Session[kTwitterUserId] = account.user_id;
            }

            return Json(account, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Info(long id, string format)
        {
            bool useDefault = true;
            OAuthAccount account = OAuthAccount.Find(c => c.Id == id).SingleOrDefault();

            if (account != null)
            {
                if (!String.IsNullOrEmpty(account.screen_name))
                {
                    useDefault = false;
                    return Json(new { Id = id, screen_name = account.screen_name, profile_image_url = account.profile_image_url }, JsonRequestBehavior.AllowGet);
                }
            }

            if (useDefault)
            {
                return Json(new
                {
                    Id = id,
                    ScreenName = "anonymous",
                    ImageUrl = VirtualPathUtility.ToAbsolute(
                    CloudSettingsResolver.GetConfigSetting("DefaultAvatarImage") as string)
                }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Image(long id, string format)
        {
            bool useDefault = true;
            OAuthAccount account = OAuthAccount.Find(c => c.Id == id).SingleOrDefault();

            if (account != null)
            {
                if (!String.IsNullOrEmpty(account.profile_image_url))
                {
                    useDefault = false;
                    Response.Redirect(account.profile_image_url);
                }
            }

            if (useDefault)
            {
                string location = VirtualPathUtility.ToAbsolute(CloudSettingsResolver.GetConfigSetting("DefaultAvatarImage") as string);
                Response.Redirect(location);
            }

            return View();
        }

        [NonAction]
        private bool TokenExpired(string oauth_token, out OAuthAccount account)
        {
            var expired = false;
            account = null;

            try
            {
                account = OAuthAccount.Find(c => c.oauth_token == oauth_token).SingleOrDefault();
                if (account != null)
                {
                    bool useTokenExpiry = false;
                    bool.TryParse(CloudSettingsResolver.GetConfigSetting("UseTokenExpiry"), out useTokenExpiry);
                    if (useTokenExpiry)
                    {
                        expired = (DateTime.UtcNow.Ticks - account.TokenExpiry.ToUniversalTime().Ticks) >= 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
            }

            return expired;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AuthorizeReturn(string oauth_token, string oauth_verifier)
        {
            this.ViewData["oauth_token"] = oauth_token;
            this.ViewData["oauth_verifier"] = oauth_verifier;

            return View();
        }

        [Prerequisite(Authenticated = true)]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Get)]
        public ActionResult UpdateTwitterStatus(string status, decimal? lat, decimal? lng)
        {
            OAuthClientApp app = OAuthClientApp.Find(c => c.Id.Equals(AuthenticatedUser.oauth_service_id)).SingleOrDefault();
            if (app == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Invalid or unknown appId" }, JsonRequestBehavior.AllowGet);
            }

            OAuthAccount account = null;
            bool tokenExpired = TokenExpired(AuthenticatedUser.oauth_token, out account);

            // Check for UserAccess
            if (account != null)
            {
                if ((DataEnums.UserAccess)account.UserAccess != DataEnums.UserAccess.Normal)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
            }

            // Check for token expiry
            if (tokenExpired)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Expired token." }, JsonRequestBehavior.AllowGet);
            }

            string postUrl = String.Format("{0}?status={1}", TwitterUpdateStatusUrl, status);
            if (lat != null && lng != null)
            {
                postUrl = String.Format("{0}&lat={1}&long={2}", postUrl, lat.Value, lng.Value);
            }

            string posted = OAuth.GetProtectedResource(
                "POST",
                postUrl,
                app.ConsumerKey,
                app.ConsumerSecret,
                AuthenticatedUser.oauth_token,
                AuthenticatedUser.oauth_token_secret);

            HttpContext.Response.StatusCode = (int)NUrl.LastResponseStatusCode.GetValueOrDefault();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> obj = serializer.DeserializeObject(posted) as Dictionary<string, object>;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}
