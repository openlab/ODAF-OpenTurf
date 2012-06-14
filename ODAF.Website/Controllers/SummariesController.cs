using System;
using System.Web.Mvc;
using System.Web.Routing;
using ODAF.Data;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using SubSonic.DataProviders;
using SubSonic.Schema;
using System.Globalization;
using website_mvc.Code;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace vancouveropendata.Controllers
{
    public class SummariesController : BaseController
    {
        const int MAX_ITEMS_PER_PAGE = 100;

        public ActionResult Index()
        {
            // This should just return the View, which should have API docs
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "MAX_ITEMS_PER_PAGE", MAX_ITEMS_PER_PAGE.ToString() }
            };

            return View(dict);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Prerequisite(Authenticated = true)]
        public ActionResult AddRating(long id, int rating, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.Id == id);
            PointDataSummary summary = iq.SingleOrDefault();

            if (summary == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Json(null);
            }

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    rating = Math.Max(Math.Min(rating, 100), 0); // between 0 and 100
                    if (rating != 0)
                    {
                        summary.RatingTotal += rating;
                        summary.RatingCount++;
                        summary.ModifiedOn = DateTime.UtcNow;
                        summary.Save();

                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(summary);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Prerequisite(Authenticated = true)]
        public ActionResult AddTag(long id, string tag, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.Id == id);
            PointDataSummary summary = iq.SingleOrDefault();

            if (summary == null)
            {
                summary = new PointDataSummary()
                {
                    Id = id,
                    Tag = ""
                };
                //HttpContext.Response.StatusCode = 404;
                //return Json(null);
            }

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    string filteredTag = filterForDuplicates(summary.Tag, tag);

                    if (!String.IsNullOrEmpty(filteredTag))
                    {
                        string fmt = String.IsNullOrEmpty(summary.Tag)? "{0}" : ",{0}";
                        summary.Tag += String.Format(fmt, filteredTag);
                        summary.ModifiedOn = DateTime.UtcNow;
                        summary.Save();
                        if (RoleEnvironment.IsAvailable)
                            SearchEngine.Instance.Index(summary.Id);
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(summary);
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Moderator)]
        public ActionResult RemoveTag(long id, string tag, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.Id == id);
            PointDataSummary summary = iq.SingleOrDefault();

            if (summary == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Json(null);
            }

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    if (!String.IsNullOrEmpty(tag))
                    {
                        // TODO: find the tag, and remove, then save

                        //string fmt = String.IsNullOrEmpty(summary.Tag) ? "{0}" : ",{0}";
                        //summary.Tag += String.Format(fmt, tag);
                        //summary.ModifiedOn = DateTime.UtcNow;

                        //summary.Save();

                        // Add summary to search index
                        //SearchEngine.Index(summary);

                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(summary);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowForCommunityByActivity(string format, int? page, int? page_size)
        {
            var iq = from summary in PointDataSummary.All()
                     where !PointDataFeed.All().Any(c => c.UniqueId == summary.LayerId)
                     orderby summary.ModifiedOn descending
                     select summary;

            if (page == null || page_size == null) {
                page = 0;
                page_size = MAX_ITEMS_PER_PAGE;
            }

            PagedList<PointDataSummary> pl = new PagedList<PointDataSummary>(iq, page.Value, page_size.Value);

            if (pl == null) {
                return Json(new List<PointDataSummary> { }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(pl.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the Summary for an id.
        /// </summary>
        /// <param name="id">the PointSummaryData id of the Summary to get</param>
        /// <param name="format">json or html</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowById(long id, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.Id == id);
            PointDataSummary summary = iq.SingleOrDefault();

            if (summary == null) {
                HttpContext.Response.StatusCode = 404;
            }

            if (format.ToLower().Equals("html")) {
                return View(summary);
            }
            else {
                return Json(summary, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the Summaries for a user (non-system layers, thus user-generated only).
        /// </summary>
        /// <param name="id">the userId to get Summaries for</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowByUserId(long id, string format)
        {
            if (id == 0) {
                id = CurrentUserId;
            }

            // get the user-gen layers
            var ie = (from a in PointDataSummary.Find(c => c.CreatedById == id) select a.LayerId)
                     .Except(from b in PointDataFeed.All() select b.UniqueId)
                     .Distinct();

            // from the user-gen layers, get the user-gen summaries
            var iq = from a in PointDataSummary.Find(c => ie.Contains(c.LayerId)) 
                     select a;

            if (iq == null) {
                return Json(new List<PointDataSummary> { }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(iq.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the Layers (non-system, thus user-generated) for a user.
        /// </summary>
        /// <param name="id">the userId to get layers for</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowLayersByUserId(long id, string format)
        {
            if (id == 0) {
                id = CurrentUserId;
            }

            var iq = (from a in PointDataSummary.Find(c => c.CreatedById == id) select a.LayerId)
                     .Except(from b in PointDataFeed.All() select b.UniqueId)
                     .Distinct();

            if (iq == null) {
                return Json(new List<string> { }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(iq.ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        
        /// <summary>
        /// Gets the Summaries for a layerId.
        /// </summary>
        /// <param name="guid">the PointSummaryData layerId of the Summaries to get</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowByLayerId(string layerId, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.LayerId == layerId);

            if (iq == null) {
                return Json(new List<PointDataSummary>(), JsonRequestBehavior.AllowGet);
            }
            else{
                return Json(iq.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the Summary for a guid.
        /// </summary>
        /// <param name="guid">the PointSummaryData guid of the Summary to get</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult ShowByGuid(string guid, string format)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                    c => c.Guid == guid);
            PointDataSummary summary = iq.SingleOrDefault();

            if (summary == null)
            {
                HttpContext.Response.StatusCode = 404;
            }

            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the Summary from three pieces of information: lat, long, and layerId
        /// </summary>
        /// <param name="lat">latitude of the summary</param>
        /// <param name="lng">longitude of the summary</param>
        /// <param name="layerId">layerId of the summary</param>
        /// <returns></returns>
        [NonAction]
        private PointDataSummary GetSummary(decimal lat, decimal lng, string layerId)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                   c => c.Latitude == lat && c.Longitude == lng && c.LayerId == layerId
                   );

            return (iq == null) ? null : iq.FirstOrDefault();
        }

        /// <summary>
        /// Gets the Summary from three pieces of information: lat, long, and layerId
        /// </summary>
        /// <param name="lat">latitude of the summary</param>
        /// <param name="lng">longitude of the summary</param>
        /// <param name="layerId">layerId of the summary</param>
        /// <returns></returns>
        [NonAction]
        private PointDataSummary GetSummary(string guid, string layerId)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                   c => c.Guid == guid && c.LayerId == layerId
                   );

            return (iq == null) ? null : iq.FirstOrDefault();
        }

        /// <summary>
        /// Gets the Summaries for search criteria
        /// </summary>
        /// <param name="format">Defaults to Json.  No other supported formats.</param>
        /// <param name="layers">Comma-delimited list of layerIds to search</param>
        /// <param name="id">Lucene search phrase. Limited to slug format.</param>
        /// <param name="term">Lucene search phrase</param>
        /// <param name="tag">Shorcut lucene search phrase for Tag field</param>
        /// <param name="name">Shorcut lucene search phrase for Name field</param>
        /// <param name="results">Max number of results to return.  Default and maximum value of 100.</param>
        /// <returns>Json array of DataPointSummary objects.</returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Search(string format, string layers = "", string id = "", string term = "", string tag = "", string name = "", int results = 100)
        {
            if (results > 100 || results <= 0) results = 100;

            // Construct the phrase for the search terms. ( terms are OR'ed )
            string query = "";
            if (!string.IsNullOrEmpty(id)) query += id + " ";
            if (!string.IsNullOrEmpty(term)) query += term + " ";
            if (!string.IsNullOrEmpty(tag)) query += "Tags:(" + tag + ") ";
            if (!string.IsNullOrEmpty(name)) query += "Name:(" + name + ") ";

            // Construct the filter phrase for the layers. ( separate layers are OR'ed )
            string filter = "";
            if (!string.IsNullOrEmpty(layers))
            {
                filter += "LayerId:(";
                foreach (string aLayer in layers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    filter += "\"" + aLayer + "\" ";
                }
                filter += ")";
            }

            // Combine query and filter into a single phrase for the lucene engine
            string phrase = "";
            if (query != "") phrase += " +(" + query + ")^2"; // boost query phrase as it's more important
            if (filter != "") phrase += " +(" + filter + ")";

            try
            {
                return Json(SearchEngine.Instance.SearchForPhrase(phrase, results), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw new Exception("Search error: " + phrase);
            }

        }

        /// <summary>
        /// Gets the Summary data from three pieces of information: lat, long, and layerId
        /// </summary>
        /// <param name="lat">latitude of the summary</param>
        /// <param name="lng">longitude of the summary</param>
        /// <param name="layerId">layerId of the summary</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Show(decimal lat, decimal lng, string layerId, string format)
        {
            PointDataSummary summary = GetSummary(lat, lng, layerId);

            if (summary == null) {
                HttpContext.Response.StatusCode = 404;
            }

            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult List(string format, int? page, int? page_size)
        {
            IQueryable<PointDataSummary> iq = PointDataSummary.All()
                .OrderByDescending(c => c.CreatedOn);

            if (page == null || page_size == null) {
                page = 0;
                page_size = MAX_ITEMS_PER_PAGE;
            }

            PagedList<PointDataSummary> pl = new PagedList<PointDataSummary>(iq, page.Value, page_size.Value);
            iq = pl.AsQueryable();

            if (iq == null) {
                return Json(new List<PointDataSummary> { }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(iq.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets a list of PointDataSummaries for a region.
        /// </summary>
        /// <param name="lat">latitude of the center point of the region</param>
        /// <param name="lng">longitude of the center point of the region</param>
        /// <param name="latDelta">delta amount of latitude from center point</param>
        /// <param name="lngDelta">delta amount of longitude from center point</param>
        /// <param name="layerId">filter by this layerId (optional)</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]       
        public ActionResult ListByRegion(decimal lat, decimal lng, decimal latDelta, decimal lngDelta, string layerId,
            string format, int? page, int? page_size)
        {
            decimal latMin = lat - latDelta;
            decimal latMax = lat + latDelta;
            decimal lngMin = lng - lngDelta;
            decimal lngMax = lng + lngDelta;

            IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                   c => (c.Latitude >= latMin &&  c.Latitude <= latMax) &&
                        (c.Longitude >= lngMin && c.Longitude <= lngMax)
                   ).OrderByDescending(c => c.CreatedOn);

            if (!String.IsNullOrEmpty(layerId)) {
                iq = iq.Where(
                    c => c.LayerId == layerId
                    );
            }

            if (page == null || page_size == null) {
                page = 0;
                page_size = MAX_ITEMS_PER_PAGE;
            }

            PagedList<PointDataSummary> pl = new PagedList<PointDataSummary>(iq, page.Value, page_size.Value);
            iq = pl.AsQueryable();

            if (iq == null) {
                return Json(new List<PointDataSummary> { }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(iq.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        private string filterForDuplicates(string currentTags, string newTags)
        {
            const char separator = ',';
            string[] newTagsArray = newTags.ToLower().Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            string[] currentTagsArray = currentTags.ToLower().Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            string filtered = String.Empty;

            foreach (string newTag in newTagsArray)
            {
                string trim = newTag.Trim();
                if (!currentTagsArray.Contains(trim))
                {
                    string fmt = String.IsNullOrEmpty(filtered) ? "{0}" : ",{0}";
                    filtered += String.Format(fmt, newTag);
                }
            }

            return filtered;
        }

        /// <summary>
        /// Adds a new Summary.
        /// Details of the summary are in the POST data.
        /// </summary>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
        [Prerequisite(Authenticated = true)]
        public ActionResult Add(string format)
        {
            PointDataSummary newSummary = null;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    newSummary = new PointDataSummary();
                    
                    // Get POST data
                    TryUpdateModel<PointDataSummary>(newSummary, new[] { "Description", "LayerId", "Latitude", "Longitude", "Guid", "Name", "Tag" });

                    if (newSummary.Latitude == 0)
                    {
                        string lat = HttpContext.Request["Latitude"];
                        newSummary.Latitude = decimal.Parse(lat, CultureInfo.InvariantCulture);
                    }

                    if (newSummary.Longitude == 0)
                    {
                        string longi = HttpContext.Request["Longitude"];
                        newSummary.Longitude = decimal.Parse(longi, CultureInfo.InvariantCulture);
                    }

                    PointDataSummary testSummary = GetSummary(newSummary.Guid, newSummary.LayerId);
                    if (testSummary != null) {
                        throw new Exception("PointDataSummary already exists in database guid, layerId");
                    }

                    newSummary.Description = newSummary.Description ?? string.Empty;
                    newSummary.Tag = newSummary.Tag ?? string.Empty;

                    newSummary.CreatedOn = DateTime.UtcNow;
                    newSummary.ModifiedOn = newSummary.CreatedOn;
                    newSummary.CreatedById = CurrentUserId;
                    newSummary.Save();
                    if (RoleEnvironment.IsAvailable)
                        SearchEngine.Instance.Index(newSummary.Id);
                    ts.Complete();

                    // Set 201 status code, and Location header
                    HttpContext.Response.StatusCode = 201;
                    string location = this.BuildUrlFromExpression<SummariesController>(c => c.ShowById(newSummary.Id, format));
                    HttpContext.Response.AddHeader("Location", location);
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(newSummary);
        }

        /// <summary>
        /// Edit a Summary.
        /// Details of the summary are in the POST data.
        /// </summary>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs( HttpVerbs.Put | HttpVerbs.Get | HttpVerbs.Post)]
        [Prerequisite(Authenticated = true)]
        public ActionResult Edit(long id, string format)
        {
            PointDataSummary summary = null;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                            c => c.Id == id);
                    summary = iq.SingleOrDefault();

                    // Get POST data
                    UpdateModel<PointDataSummary>(summary, new[] {"Name", "Description", "LayerId", "Latitude", "Longitude", "Tag", "Guid" });

                    summary.ModifiedOn = DateTime.UtcNow;

                    summary.Save();
                    if(RoleEnvironment.IsAvailable)
                        SearchEngine.Instance.Index(summary.Id);
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 404;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(summary);
        }

        /// <summary>
        /// Removes a Comment.
        /// </summary>
        /// <param name="id">This is the PointSummaryData id to remove from the database</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete | HttpVerbs.Get)]
        [Prerequisite(Authenticated = true)]
        public ActionResult Remove(long id, string format)
        {
            PointDataSummary summary = null;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                            c => c.Id == id);
                    summary = iq.SingleOrDefault();
                    if (summary == null)
                    {
                        throw new Exception("PointDataSummary does not exist in database.");
                    }

                    if (summary.CreatedById != AuthenticatedUser.Id)
                    {
                        HttpContext.Response.StatusCode = 403;
                        return Json(CreateErrorObject(new Exception("Access to resource denied.")), JsonRequestBehavior.AllowGet);
                    }

                    List<PointDataComment> comments = summary.PointDataComments.ToList();
                    foreach (PointDataComment comment in comments)
                    {
                        comment.Delete();
                    }

                    summary.Delete();

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 404;
                    return Json(CreateErrorObject(ex));
                }
            }

            return Json(summary, JsonRequestBehavior.AllowGet);
        }
    }
}