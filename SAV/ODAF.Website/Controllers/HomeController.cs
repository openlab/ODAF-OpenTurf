using Microsoft.Owin.Security;
using ODAF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Twitter;
using System.Configuration;
using TweetSharp;
using System.Globalization;
using ODAF.Website.Helpers;

namespace ODAF.Website.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class HomeController : Controller
    {
        #region views
        /// <summary>
        /// Default View + Load Region List
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            CheckTwitterConnectedUser();


            List<SelectListItem> RegionList = new List<SelectListItem>();
            List<PointDataSource> DataSourceList = new List<PointDataSource>();

            var query = PointDataSource.All();
            int i = 0;
            foreach (PointDataSource data in query)
            {
                RegionList.Add(
                   new SelectListItem()
                   {
                       Text = data.Title,
                       Value = i.ToString()
                   }
                   );
                i++;
            }
            ViewData.Add("RegionList", RegionList);
            ModelState.Clear();
            ViewBag.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            ViewBag.BingCredential = ConfigurationManager.AppSettings["BingCredential"];
            return View(ViewData);
        }

        /// <summary>
        /// User authentication check for Twitter
        /// </summary>
        private void CheckTwitterConnectedUser()
        {
            var ctx = Request.GetOwinContext();
            var authenticatedUser = ctx.Authentication.User;
            var result = ctx.Authentication.AuthenticateAsync(UserAccountType.Twitter).Result;

            if (authenticatedUser.Identity.IsAuthenticated)
            {
                if (authenticatedUser.Identity.AuthenticationType == UserAccountType.Twitter)
                {

                    string screenname = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "urn:twitter:screenname").Value;
                    string socialid = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "urn:twitter:userid").Value;
                    string accessToken = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "urn:twitter:accesstoken").Value;
                    string accessTokenSecret = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "urn:twitter:accesstokensecret").Value;
                    string useravatar = GetTwitterProfileImage(accessToken, accessTokenSecret);

                    try
                    {
                        OAuthAccount account = OAuthAccount.SingleOrDefault(p => p.user_id == long.Parse(socialid));
                        if (account != null)
                        {
                            account.screen_name = screenname;
                            account.oauth_token = accessToken;
                            account.oauth_token_secret = accessTokenSecret;
                            account.profile_image_url = useravatar;
                            account.LastAccessedOn = DateTime.UtcNow;
                            account.TokenExpiry = result.Properties.ExpiresUtc.Value.DateTime;
                            account.Update();
                            ViewBag.id = account.Id;
                        }
                        else
                        {
                            OAuthAccount newAccount = new OAuthAccount();
                            newAccount.user_id = long.Parse(socialid);
                            newAccount.screen_name = screenname;
                            newAccount.oauth_token = accessToken;
                            newAccount.oauth_token_secret = accessTokenSecret;
                            newAccount.profile_image_url = useravatar;
                            newAccount.CreatedOn = DateTime.UtcNow;
                            newAccount.LastAccessedOn = DateTime.UtcNow;
                            newAccount.TokenExpiry = result.Properties.ExpiresUtc.Value.DateTime;
                            newAccount.oauth_service_id = 1;
                            newAccount.UserAccess = 0;
                            newAccount.UserRole = 0;
                            newAccount.Save();
                            ViewBag.id = account.Id;
                        }
                    }
                    catch (Exception) { }

                    ViewBag.screenname = screenname;
                    ViewBag.socialid = socialid;
                    ViewBag.accessToken = accessToken;
                    ViewBag.accessTokenSecret = accessTokenSecret;
                    ViewBag.useravatar = useravatar;
                    ViewBag.Logged = true;
                }
                else
                {
                    ViewBag.Logged = false;
                    ctx.Authentication.SignOut(authenticatedUser.Identity.AuthenticationType);
                    Redirect("/");
                }
            }
            else
            {
                ViewBag.Logged = false;
                ViewBag.screenname = null;
                ViewBag.socialid = null;
                ViewBag.id = null;
            }
        }
        #endregion


        #region Data
        /// <summary>
        /// Get the list of available Regions
        /// </summary>
        /// <returns>List of region in json format</returns>
        public ActionResult Regions()
        {
            List<PointDataSource> DataSourceList = new List<PointDataSource>();
            var query = PointDataSource.All();
            foreach (PointDataSource data in query)
                DataSourceList.Add(data);
            return Json(DataSourceList);
        }

        /// <summary>
        /// Get the list of Feed for a Region
        /// </summary>
        /// <param name="Region_Index">Index of the Region</param>
        /// <returns>List of feed in json format</returns>
        public ActionResult Feeds(int Region_Index = 0)
        {
            List<PointDataSource> DataSourceList = new List<PointDataSource>();
            var query = PointDataSource.All();
            foreach (PointDataSource data in query)
                DataSourceList.Add(data);

            var feed = DataSourceList[Region_Index].PointDataFeeds.Select(e => e).Where(e => e.Active == true);
            List<PointDataFeed> FeedList = new List<PointDataFeed>();

            foreach (PointDataFeed f in feed)
                FeedList.Add(f);
            return Json(FeedList);


        }

        /// <summary>
        /// Get the url to call to get the list of Point of a Feed
        /// </summary>
        /// <param name="Region_Index">Index of the Region</param>
        /// <param name="Feed_Index">Index of the Feed</param>
        /// <returns>Url to call to get the list of point</returns>
        public string GetPointsFeedUrl(int Region_Index = 0, int Feed_Index = 0)
        {
            List<PointDataSource> DataSourceList = new List<PointDataSource>();
            var query = PointDataSource.All();
            foreach (PointDataSource data in query)
                DataSourceList.Add(data);

            var feed = DataSourceList[Region_Index].PointDataFeeds.Select(e => e);
            List<PointDataFeed> FeedList = new List<PointDataFeed>();

            foreach (PointDataFeed f in feed)
                FeedList.Add(f);
            return FeedList[Feed_Index].KMLFeedUrl.Replace("kml", "json");
        }
        #endregion

        #region Summaries
        public ActionResult GetSharedLandmarks(bool showlandmarks, bool showuserlandmarks, string userid)
        {
            if ((showlandmarks && showuserlandmarks) || (showlandmarks && !showuserlandmarks && userid == null))
            {
                var query = from summary in PointDataSummary.All()
                            where !PointDataFeed.All().Any(c => c.UniqueId == summary.LayerId)
                            orderby summary.ModifiedOn descending
                            select summary;
                var summaries = from s in query
                                join a in OAuthAccount.All() on s.CreatedById equals a.Id
                                select new { Summary = s, Avatar = a.profile_image_url };
                var placemarklist = summaries.ToList();
                return Json(placemarklist, JsonRequestBehavior.AllowGet);
            }
            else if (showlandmarks && !showuserlandmarks && userid != null)
            {
                var query = from summary in PointDataSummary.All()
                            where !PointDataFeed.All().Any(c => c.UniqueId == summary.LayerId)
                            where summary.LayerId != userid
                            orderby summary.ModifiedOn descending
                            select summary;
                var summaries = from s in query
                                join a in OAuthAccount.All() on s.CreatedById equals a.Id
                                select new { Summary = s, Avatar = a.profile_image_url };
                var placemarklist = summaries.ToList();
                return Json(placemarklist, JsonRequestBehavior.AllowGet);
            }
            else if (!showlandmarks && showuserlandmarks)
            {
                var query = from summary in PointDataSummary.All()
                            where !PointDataFeed.All().Any(c => c.UniqueId == summary.LayerId)
                            where summary.LayerId == userid
                            orderby summary.ModifiedOn descending
                            select summary;
                var summaries = from s in query
                                join a in OAuthAccount.All() on s.CreatedById equals a.Id
                                select new { Summary = s, Avatar = a.profile_image_url };
                var placemarklist = summaries.ToList();
                return Json(placemarklist, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLandmark(string name, string description, string layerid, string latitude, string longitude, string createdby)
        {
            try
            {
                PointDataSummary newSummary = new PointDataSummary()
                {
                    Name = name,
                    Description = description,
                    LayerId = layerid,
                    Latitude = decimal.Parse(latitude, CultureInfo.InvariantCulture),
                    Longitude = decimal.Parse(longitude, CultureInfo.InvariantCulture),
                    Guid = Guid.NewGuid().ToString(),
                    CreatedById = long.Parse(createdby, CultureInfo.InvariantCulture),
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow
                };
                newSummary.Save();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSummaryByGuid(string guid, string name, string latitude, string longitude, string createdby, int Region_Index = 0, int Feed_Index = 0)
        {
            PointDataSummary summary = PointDataSummary.All().FirstOrDefault(s => s.Guid == guid);

            if (summary == null)
            {
                summary = new PointDataSummary()

                {
                    Name = name,
                    Description = name,
                    LayerId = GetFeedUniqueId(Region_Index, Feed_Index),
                    Latitude = decimal.Parse(latitude, CultureInfo.InvariantCulture),
                    Longitude = decimal.Parse(longitude, CultureInfo.InvariantCulture),
                    Guid = Guid.NewGuid().ToString(),
                    CreatedById = long.Parse(createdby.Length > 0 ? createdby : "0"),
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow
                };
                if (createdby.Length > 0)
                    summary.Save();
            }

            var comments = from c in PointDataCommentView.All().Where(c => c.SummaryId == summary.Id)
                           join a in OAuthAccount.All() on c.CreatedById equals a.Id
                           select new { Text = c.Text, User = a.screen_name, Date = c.CreatedOn.ToString("dd MMM yyy hh:mm") };

            return Json(new { summary, comments }, JsonRequestBehavior.AllowGet);
        }

        public string GetFeedUniqueId(int Region_Index = 0, int Feed_Index = 0)
        {
            List<PointDataSource> DataSourceList = new List<PointDataSource>();
            var query = PointDataSource.All();
            foreach (PointDataSource data in query)
                DataSourceList.Add(data);

            var feed = DataSourceList[Region_Index].PointDataFeeds.Select(e => e);
            List<PointDataFeed> FeedList = new List<PointDataFeed>();

            foreach (PointDataFeed f in feed)
                FeedList.Add(f);
            return FeedList[Feed_Index].UniqueId;
        }

        public string AddTagToSummary(string guid, string name)
        {
            string tag = "";
            PointDataSummary summary = PointDataSummary.All().FirstOrDefault(s => s.Guid == guid);
            if (summary != null)
            {
                tag += summary.Tag;
                tag += (tag.Length > 0) ? ", " : "";
                tag += name;
                summary.Tag = tag;
                summary.Save();
                tag = summary.Tag;
            }
            return tag;
        }

        public ActionResult AddCommentToSummary(string Text, string CreatedBy, string SummaryId)
        {
            PointDataComment comment = new PointDataComment()
            {
                Text = Text,
                CreatedOn = DateTime.UtcNow,
                CreatedById = long.Parse(CreatedBy),
                SummaryId = long.Parse(SummaryId)
            };

            comment.Save();

            PointDataSummary summary = PointDataSummary.All().FirstOrDefault(s => s.Id == long.Parse(SummaryId));
            summary.CommentCount += 1;
            summary.Save();

            return Json(new { Text = comment.Text, Date = comment.CreatedOn.ToString("dd MMM yyy hh:mm") }, JsonRequestBehavior.AllowGet);
        }

        public string AddRateToSummary(string guid, string rate)
        {
            string message = Resources.Views.Home.Index.StringRateError;
            PointDataSummary summary = PointDataSummary.All().FirstOrDefault(s => s.Guid == guid);
            if (summary != null)
            {
                int _rate = int.Parse(rate);
                summary.RatingCount += 1;
                summary.RatingTotal += (_rate * 20);
                summary.Save();

                message = Resources.Views.Home.Index.StringRated + " " + _rate + " " + Resources.Views.Home.Index.StringStar + ((_rate > 1) ? "s" : "");
            }

            return message;
        }

        public string DeleteSummary(string guid, string userId)
        {
            PointDataSummary summary = PointDataSummary.All().FirstOrDefault(s => s.Guid == guid && s.CreatedById == long.Parse(userId));
            if (summary != null)
            {
                PointDataComment.Delete(e => e.SummaryId == summary.Id);
                summary.Delete();
            }

            return null;
        }
        #endregion

        #region Twitter
        /// <summary>
        /// Call the Twitter Authentication
        /// </summary>
        /// <returns></returns>
        public ActionResult Twitter()
        {
            var provider = UserAccountType.Twitter;
            var ctx = Request.GetOwinContext();
            ctx.Authentication.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Callback", new { provider })
                },
                provider);
            return new HttpUnauthorizedResult();
        }

        /// <summary>
        /// Callback method for the Twitter Authentication
        /// </summary>
        /// <param name="provider">Name of the authentication provider</param>
        /// <returns></returns>
        public ActionResult Callback(string provider)
        {
            var ctx = Request.GetOwinContext();
            var result = ctx.Authentication.AuthenticateAsync(UserAccountType.External).Result;
            ctx.Authentication.SignOut(UserAccountType.External);

            if (result != null)
            {
                var claims = result.Identity.Claims.ToList();
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, provider));
                var ci = new ClaimsIdentity(claims, UserAccountType.Twitter);
                ctx.Authentication.SignIn(ci);
            }
            return Redirect("~/");
        }

        /// <summary>
        /// Forget the connected twitter user
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            Request.GetOwinContext().Authentication.SignOut(UserAccountType.Twitter);
            return Redirect("~/");
        }

        /// <summary>
        /// Retreive the url of the Connected Twitter Users's profile picture
        /// </summary>
        /// <param name="accessToken">Twitter user's Access Token</param>
        /// <param name="accessTokenSecret">Twitter user's Access Token Secret</param>
        /// <returns>Url of the Twitter Users's profile picture</returns>
        private string GetTwitterProfileImage(string accessToken, string accessTokenSecret)
        {
            if (accessToken != null && accessTokenSecret != null)
            {
                // Initialize the Twitter client
                var service = new TwitterService(
                    ConfigurationManager.AppSettings["TwitterConsumerKey"],
                    ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                    accessToken,
                    accessTokenSecret
                    );
                var profile = service.GetUserProfile(new GetUserProfileOptions());

                if (profile != null && !String.IsNullOrWhiteSpace(profile.ProfileImageUrlHttps))
                {
                    return profile.ProfileImageUrlHttps;
                }
                return "";
            }
            return null;
        }
        #endregion
    }
}