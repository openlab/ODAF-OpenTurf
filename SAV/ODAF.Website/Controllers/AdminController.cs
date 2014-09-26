using Microsoft.Owin.Security;
using Microsoft.Owin.Security.WsFederation;
using ODAF.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ODAF.Website.Controllers
{
    //[Authorize ]
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class AdminController : Controller
    {
        #region views
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var ctx = Request.GetOwinContext();
            var authenticatedUser = ctx.Authentication.User;


            if (authenticatedUser.Identity.IsAuthenticated && authenticatedUser.Identity.AuthenticationType == WsFederationAuthenticationDefaults.AuthenticationType)
            {
                var givenName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
                var surname = authenticatedUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
                ViewBag.AdminFullName = string.Format("{0} {1}", givenName, surname);
                ;
            }
            else
            {
                ctx.Authentication.SignOut(authenticatedUser.Identity.AuthenticationType);
                ctx.Authentication.Challenge(WsFederationAuthenticationDefaults.AuthenticationType);
                return new HttpUnauthorizedResult();
            }


            return View();
        }

        //
        // GET: /Admin/SignOut
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut(WsFederationAuthenticationDefaults.AuthenticationType);
            return Redirect("/");
        }
        #endregion

        #region users
        public ActionResult GetUsers()
        {
            var query = OAuthAccountView.All().Where(p => p.Id > 0);
            List<OAuthAccountView> userlist = new List<OAuthAccountView>();
            foreach (OAuthAccountView o in query)
            {
                userlist.Add(o);
            }
            return Json(userlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUser(string id, string useraccess, string userrole)
        {
            try
            {
                OAuthAccount user = OAuthAccount.SingleOrDefault(p => p.Id == int.Parse(id));
                if (user == null)
                    return Json("not found", JsonRequestBehavior.AllowGet);
                user.UserAccess = short.Parse(useraccess);
                user.UserRole = short.Parse(userrole);
                user.Update();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region sources
        public ActionResult GetSources()
        {
            var query = PointDataSourceView.All();
            List<PointDataSourceView> sourcelist = new List<PointDataSourceView>();
            foreach (PointDataSourceView s in query)
            {
                sourcelist.Add(s);
            }
            return Json(sourcelist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateSource(string title, string updated, string description, string authorname, string authoremail, string boundary, string active)
        {
            try
            {
                var newSource = new PointDataSource()
                {
                    Title = title,
                    UpdatedOn = DateTime.Parse(updated),
                    Description = description,
                    AuthorName = authorname,
                    AuthorEmail = authoremail,
                    BoundaryPolygon = boundary,
                    Active = bool.Parse(active),
                    CreatedOn = DateTime.UtcNow,
                    UniqueId = Guid.NewGuid().ToString()
                };
                newSource.Save();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteSource(string id)
        {
            try
            {
                PointDataFeed.Delete(p => p.PointDataSourceId == int.Parse(id));
                PointDataSource.Delete(p => p.PointDataSourceId == int.Parse(id));
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateSource(string id, string title, string updated, string description, string authorname, string authoremail, string boundary, string active)
        {

            try
            {
                PointDataSource source = PointDataSource.SingleOrDefault(p => p.PointDataSourceId == long.Parse(id));
                if (source == null)
                    return Json("not found", JsonRequestBehavior.AllowGet);
                source.Title = title;
                source.UpdatedOn = DateTime.Parse(updated);
                source.Description = description;
                source.AuthorName = authorname;
                source.AuthorEmail = authoremail;
                source.BoundaryPolygon = boundary;
                source.Active = bool.Parse(active);
                source.Update();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region feeds
        public ActionResult GetFeeds()
        {
            var query = PointDataFeedView.All();
            List<PointDataFeedView> feedlist = new List<PointDataFeedView>();
            foreach (PointDataFeedView s in query)
            {
                feedlist.Add(s);
            }
            return Json(feedlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateFeed(string title, string pointdatasourceid, string updated, string summary, string kml, string image, string region, string active)
        {
            try
            {
                var newfeed = new PointDataFeed()
                {
                    Title = title,
                    UpdatedOn = DateTime.Parse(updated),
                    PointDataSourceId = long.Parse(pointdatasourceid),
                    Summary = summary,
                    KMLFeedUrl = kml,
                    ImageUrl = image,
                    IsRegion = bool.Parse(region),
                    Active = bool.Parse(active),
                    CreatedOn = DateTime.UtcNow,
                    UniqueId = Guid.NewGuid().ToString()
                };
                newfeed.Save();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteFeed(string id)
        {
            try
            {
                PointDataFeed.Delete(p => p.PointDataFeedId == int.Parse(id));
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateFeed(string id, string title, string pointdatasourceid, string updated, string summary, string kml, string image, string region, string active)
        {
            try
            {
                PointDataFeed feed = PointDataFeed.SingleOrDefault(p => p.PointDataFeedId == long.Parse(id));
                if (feed == null)
                    return Json("not found", JsonRequestBehavior.AllowGet);
                feed.Title = title;
                feed.UpdatedOn = DateTime.Parse(updated);
                feed.PointDataSourceId = long.Parse(pointdatasourceid);
                feed.Summary = summary;
                feed.KMLFeedUrl = kml;
                feed.ImageUrl = image;
                feed.IsRegion = bool.Parse(region);
                feed.Active = bool.Parse(active);
                feed.Update();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Placemarks

        public ActionResult GetPlacemarks()
        {
            var query = PointDataSummaryView.All();
            List<PointDataSummaryView> placemarklist = new List<PointDataSummaryView>();
            foreach (PointDataSummaryView s in query)
            {
                placemarklist.Add(s);
            }
            return Json(placemarklist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletePlacemark(string id)
        {
            try
            {
                PointDataComment.Delete(p => p.SummaryId == int.Parse(id));
                PointDataSummary.Delete(p => p.Id == int.Parse(id));
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePlacemark(string id, string layerid, string name, string description, string latitude, string longitude, string tag)
        {
            try
            {
                PointDataSummary placemark = PointDataSummary.SingleOrDefault(p => p.Id == int.Parse(id));
                if (placemark == null)
                    return Json("not found", JsonRequestBehavior.AllowGet);
                placemark.LayerId = layerid;
                placemark.Name = name;
                placemark.Description = description;
                placemark.Latitude = decimal.Parse(latitude);
                placemark.Longitude = decimal.Parse(longitude);
                placemark.Tag = tag;
                placemark.ModifiedOn = DateTime.Now;
                placemark.Update();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Comments
        public ActionResult GetComments()
        {
            var query = PointDataCommentView.All();
            List<PointDataCommentView> commentlist = new List<PointDataCommentView>();
            foreach (PointDataCommentView s in query)
            {
                commentlist.Add(s);
            }
            return Json(commentlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteComment(string id)
        {
            try
            {
                PointDataComment.Delete(p => p.Id == int.Parse(id));
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}