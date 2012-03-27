using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using ODAF.Data;
using SubSonic.DataProviders;
using SubSonic.Schema;
using website_mvc.Code;

namespace vancouveropendata.Controllers
{
    public class CommentsController : BaseController
    {
        public ActionResult Index()
        {
            // This should just return the View, which should have API docs
            return View();
        }


        private T Cast<T>(object obj, T type)
        {
            return (T)obj;
        }

        /// <summary>
        /// Gets the AggregateComment for an id.
        /// </summary>
        /// <param name="id">the PointCommentData id of the Comment to get</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Show(long id, string format)
        {
            IQueryable<PointDataComment> iq = PointDataComment.All().Where(
                    c => c.Id == id);

            var aggregate = (from comment in iq
                            join account in OAuthAccount.All() on comment.CreatedById equals account.Id
                            join clientApp in OAuthClientApp.All() on account.oauth_service_id equals clientApp.Id
                            select new { Comment = comment, CommentAuthor = account.screen_name, ServiceName = clientApp.oauth_service_name })
                            ;
            
            var result = aggregate.SingleOrDefault();
            if (result == null)
            {
                HttpContext.Response.StatusCode = 404;
            }

            if (format.ToLower().Equals("html"))
            {
                // fastest way to convert anon type to defined
                AggregatedComment comment = result != null ? new AggregatedComment { 
                    Comment = result.Comment, CommentAuthor = result.CommentAuthor, ServiceName = result.ServiceName } : null;
                return View(comment);
            }
            else
            {
                return Json(result,JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Lists AggregateComments for a particular PointDataSummary.
        /// </summary>
        /// <param name="id">This is the PointSummaryData id to get comments for</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <param name="page">the page index of results to get</param>
        /// <param name="page_size">the number of results per page to get</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult List(long id, string format, int? page, int? page_size)
        {
            IQueryable<PointDataSummary> iq_s = PointDataSummary.All().Where(
                    c => c.Id == id);
            PointDataSummary summary = iq_s.FirstOrDefault();

            if (summary == null) 
            {
                HttpContext.Response.StatusCode = 404;
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                IQueryable<PointDataComment> iq_c = summary.PointDataComments.OrderByDescending(c => c.CreatedOn);

                // Get paged values, if available
                if (page != null && page_size != null)
                {
                    PagedList<PointDataComment> pl = new PagedList<PointDataComment>(iq_c, page.Value, page_size.Value);
                    iq_c = pl.AsQueryable();
                }

                var aggregate = from p in iq_c
                                join a in OAuthAccount.All() on p.CreatedById equals a.Id
                                join b in OAuthClientApp.All() on a.oauth_service_id equals b.Id
                                select new { Comment = p, CommentAuthor = a.screen_name, ServiceName = b.oauth_service_name };

                return Json(aggregate.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Adds a new Comment.
        /// Details of the comment are in the POST data.
        /// </summary>
        /// <param name="id">This is the PointSummaryData id to add the comment to</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
        [Prerequisite(Authenticated = true)]
        public ActionResult Add(long id, string format)
        {
            PointDataComment newComment = null;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    // Modify the summary
                    IQueryable<PointDataSummary> iq = PointDataSummary.All().Where(
                            c => c.Id == id);
                    PointDataSummary summary = iq.FirstOrDefault();

                    summary.ModifiedOn = DateTime.UtcNow;
                    summary.CommentCount += 1;

                    summary.Save();

                    // Save the comment
                    newComment = new PointDataComment();

                    // Get POST data
                    UpdateModel<PointDataComment>(newComment, new[] { "Text" });

                    newComment.SummaryId = id;
                    newComment.CreatedById = CurrentUserId;
                    //newComment.CreatedOn = summary.ModifiedOn;

                    newComment.Save();
                    
                    ts.Complete();


                    //retVal = aggregate;

                    // Set 201 status code, and Location header
                    HttpContext.Response.StatusCode = 201;
                    string location = this.BuildUrlFromExpression<CommentsController>(c => c.Show(newComment.Id, format));
                    HttpContext.Response.AddHeader("Location", location);
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            var aggregate = (from a in OAuthAccount.All()
                             where a.Id == newComment.CreatedById
                             join b in OAuthClientApp.All() on a.oauth_service_id equals b.Id
                             select new { Comment = newComment, CommentAuthor = a.screen_name, ServiceName = b.oauth_service_name })
                .SingleOrDefault();

            return Json(aggregate);
        }

        /// <summary>
        /// Updates a Comment.
        /// </summary>
        /// <param name="id">This is the PointSummaryData id to add the comment to</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Moderator)]
        public ActionResult Edit(long id, string format)
        {
            PointDataComment comment = null;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    IQueryable<PointDataComment> iq = PointDataComment.All().Where(
                            c => c.Id == id);
                    comment = iq.FirstOrDefault();

                    // Get POST data
                    UpdateModel<PointDataComment>(comment, new[] { "Text" });

                    comment.Save();

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(comment);
        }

        /// <summary>
        /// Removes a Comment.
        /// </summary>
        /// <param name="id">This is the PointCommentData id to remove from the database</param>
        /// <param name="format">Currently, only "json" is supported</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult Remove(long id, string format)
        {
            PointDataComment comment = null;;

            using (TransactionScope ts = new TransactionScope())
            using (SharedDbConnectionScope sharedConnectionScope = new SharedDbConnectionScope())
            {
                try
                {
                    IQueryable<PointDataComment> iq = PointDataComment.All().Where(
                            c => c.Id == id);
                    comment = iq.SingleOrDefault();
                    PointDataSummary summary = comment.PointDataSummaries.FirstOrDefault();

                    comment.Delete();

                    summary.ModifiedOn = DateTime.UtcNow;
                    summary.CommentCount -= 1;

                    summary.Save();

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json( CreateErrorObject(ex) );
                }
            }

            return Json(comment);
        }
    
    }
}