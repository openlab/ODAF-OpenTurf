using System;
using System.Web.Mvc;
using System.Web.Routing;
using ODAF.Data;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using SubSonic.DataProviders;
using SubSonic.Schema;
using System.Linq.Expressions;

namespace vancouveropendata.Controllers
{

    

    public class AdminController : BaseController
    {

        #region models
       
        public class jqGridData<T> where T : class
        {
            public int total { get; set; }
            public int page { get; set; }
            public int records { get; set; }
            public IList<jqGridRow<T>> rows { get; set; }

            public jqGridData(IList<T> list, Func<T, object> idMember, Func<T, object[]> columns)
            {
                rows = list.Select(i => new jqGridRow<T>(i, idMember, columns)).ToList();
            }
        }

        public class jqGridRow<T> where T : class
        {
            public string id { get; set; }
            public string[] cell { get; set; }

            public jqGridRow(T rowInstance, Func<T, object> idMember, Func<T, object[]> columns)
            {
                id = idMember(rowInstance).ToString();
                cell = columns(rowInstance).Select(c => c.ToString()).ToArray();
            }
        }

        public IOrderedQueryable<T> Order<T>(IQueryable<T> query, string field, string direction)
        {
            var x = Expression.Parameter(query.ElementType, "p");
            var selector = Expression.Lambda(Expression.PropertyOrField(x, field), x);
            string operation = "OrderByDescending";
            if (direction == "asc")
                operation = "OrderBy";
            IOrderedQueryable<T> result = query.Provider.CreateQuery(
                 Expression.Call(typeof(Queryable), operation,
                      new Type[] { query.ElementType, selector.Body.Type },
                       query.Expression, selector)
                 ) as IOrderedQueryable<T>;
            return result;
        }

        public class jqGridModel
        {
            public int Page { get; set; }
            public int Rows { get; set; }
            public string SIdx { get; set; }
            public string SOrd { get; set; }
            public bool _Search { get; set; }
        }

        public class UsersModel : jqGridModel
        {

            public UsersModel()
            {
                Screen_Name = "";
                UserRole = -1;
                UserAccess = -1;
            }

            public string Screen_Name { get; set; }
            public int UserAccess { get; set; }
            public int UserRole { get; set; }
        }

        public class FeedsModel : jqGridModel
        {
            public int DataSource { get; set; }
        }

        public class CommentsModel : jqGridModel
        {

            public CommentsModel()
            {
                Screen_Name = "";
                Summary = "";
                Text = "";
            }

            public string Text { get; set; }
            public string Summary { get; set; }
            public string Screen_Name { get; set; }
        }

        public class PointsModel : jqGridModel
        {

            public PointsModel()
            {
                Screen_Name = "";
                Name = "";
            }

            public string Name { get; set; }
            public string Screen_Name { get; set; }
        }

        #endregion

        #region views
        
        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage=true)]
        public ActionResult Index()
        {
            // This should just return the View, which should have Test functions
            return View("Users");
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult Sources()
        {
            return View();
        }

        [Prerequisite(Authenticated = false)]
        public ActionResult AccessDeniedToRole()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

        [Prerequisite(Authenticated = false)]
        public ActionResult AccessDenied()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult ReIndex()
        {
            SearchEngine.Instance.Index(0);
            return View();
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult Feeds(int id=0)
        {
            var query=PointDataSource.All().OrderBy(p=>p.Title);
            string result = "";
            foreach (PointDataSource data in query)
            {
                result += data.PointDataSourceId+":"+data.Title+";";
            }
            ViewData.Add("DataSources", result.Remove(result.Length-1));
            ViewData.Add("DataSourceId", id);
            return View("Feeds", ViewData);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult Layers(int id = 0)
        {
            var query = PointDataSource.All().OrderBy(p => p.Title);
            string result = "";
            foreach (PointDataSource data in query)
            {
                result += data.PointDataSourceId + ":" + data.Title + ";";
            }
            ViewData.Add("DataSources", result.Remove(result.Length - 1));
            ViewData.Add("DataSourceId", id);
            return View("Layers", ViewData);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult Comments()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage=true)]
        public ActionResult Users()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator, ShowPage = true)]
        public ActionResult Points()
        {
            // This should just return the View, which should have Test functions
            return View();
        }
        
        #endregion

        #region sources

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult GetSources()
        {
            jqGridModel model = new jqGridModel();
            UpdateModel(model, new string[] { "Page", "Rows", "SIdx", "SOrd" });
            var query = PointDataSourceView.All();

            var totalCount = query.Count();
            query = Order<PointDataSourceView>(query, model.SIdx, model.SOrd).ThenByDescending(p => p.PointDataSourceId);
            var sources = query.Skip((model.Page - 1) * model.Rows).Take(model.Rows);
            var gridData = new jqGridData<PointDataSourceView>(
                sources.ToList(),
                t => t.PointDataSourceId,
                t => new object[]
                       {
                           t.PointDataSourceId,
                           t.UniqueId,
                           t.Title,
                           t.UpdatedOn.ToString("dd-MMM-yyyy HH:mm"),
                           t.Description,
                           t.AuthorName,
                           t.AuthorEmail,
                           t.BoundaryPolygon,
                           t.Active,
                           t.Feeds,
                           t.Layers
                       })
            {
                page = model.Page,
                total = (int)Math.Ceiling(totalCount / (model.Rows * 1.0)),
                records = totalCount
            };

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult UpdateSource(string id)
        {
            if (Request["oper"] == "edit")
            {
                PointDataSource source = PointDataSource.SingleOrDefault(p => p.PointDataSourceId == int.Parse(id));
                UpdateModel(source, new string[] { "UniqueId", "Title", "AuthorName", "AuthorEmail", "BoundaryPolygon", "Description", "UpdatedOn", "Active" });
                source.Update();
                return Json(source);
            }
            else if (Request["oper"] == "add")
            {
                PointDataSource source = new PointDataSource();
                UpdateModel(source, new string[] { "Title", "AuthorName", "AuthorEmail", "BoundaryPolygon", "Active", "Description", "UpdatedOn" });
                source.CreatedOn = DateTime.UtcNow;
                source.UniqueId = Guid.NewGuid().ToString();
                source.Add();
                return Json(source);
            }
            else if (Request["oper"] == "del")
            {
                PointDataFeed.Delete(p => p.PointDataSourceId == int.Parse(id));
                PointDataSource.Delete(p => p.PointDataSourceId == int.Parse(id));
                return Json(true);
            }

            throw new Exception("Bad operation.");
            
        }


        #endregion

        #region feeds

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult GetFeeds()
        {
            FeedsModel model = new FeedsModel();
            UpdateModel(model, new string[] { "Page", "Rows", "SIdx", "SOrd", "_Search", "DataSource" });
            var query = PointDataFeedView.All();
            if (model._Search)
            {
                if (model.DataSource >0) query = query.Where(p => p.PointDataSourceId==model.DataSource);
            }
            var totalCount = query.Count();
            query = Order<PointDataFeedView>(query, model.SIdx, model.SOrd).ThenByDescending(p => p.PointDataFeedId);
            var sources = query.Skip((model.Page - 1) * model.Rows).Take(model.Rows);
            var gridData = new jqGridData<PointDataFeedView>(
                sources.ToList(),
                t => t.PointDataFeedId,
                t => new object[]
                       {
                           t.DataSourceName,
                           t.PointDataSourceId,
                           t.UniqueId,
                           t.Title,
                           t.UpdatedOn.ToString("dd-MMM-yyyy HH:mm"),
                           t.Summary,
                           t.KMLFeedUrl,
                           t.ImageUrl,
                           t.IsRegion,
                           t.Active,
                       })
            {
                page = model.Page,
                total = (int)Math.Ceiling(totalCount / (model.Rows * 1.0)),
                records = totalCount
            };

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult UpdateFeed(string id)
        {
            if (Request["oper"] == "edit")
            {
                PointDataFeed feed = PointDataFeed.SingleOrDefault(p => p.PointDataFeedId == int.Parse(id));
                UpdateModel(feed, new string[] { "PointDataSourceId", "UniqueId", "Title", "Summary", "KMLFeedUrl", "ImageUrl", "IsRegion", "UpdatedOn", "Active" });
                feed.Update();
                return Json(feed);
            }
            else if (Request["oper"] == "add")
            {
                PointDataFeed feed = new PointDataFeed();
                UpdateModel(feed, new string[] { "PointDataSourceId", "Title", "Summary", "KMLFeedUrl", "ImageUrl", "IsRegion", "UpdatedOn", "Active" });
                feed.CreatedOn = DateTime.UtcNow;
                feed.UniqueId = Guid.NewGuid().ToString();
                feed.Add();
                return Json(feed);
            }
            else if (Request["oper"] == "del")
            {
                PointDataFeed.Delete(p => p.PointDataFeedId == int.Parse(id));
                return Json(true);
            }

            throw new Exception("Bad operation.");
        }

        #endregion

        #region points

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult GetPoints()
        {
            PointsModel model = new PointsModel();
            UpdateModel(model, new string[] { "Page", "Rows", "SIdx", "SOrd", "_Search", "Screen_Name", "Name" });
            var query = PointDataSummaryView.All();

            if (model._Search)
            {
                if (model.Screen_Name.Trim() != "") query = query.Where(p => p.screen_name.Contains(model.Screen_Name));
                if (model.Name.Trim() != "") query = query.Where(p => p.Name.Contains(model.Name));
            }

            var totalCount = query.Count();
            query = Order<PointDataSummaryView>(query, model.SIdx, model.SOrd).ThenByDescending(p => p.Id);
            var sources = query.Skip((model.Page - 1) * model.Rows).Take(model.Rows);
            var gridData = new jqGridData<PointDataSummaryView>(
                sources.ToList(),
                t => t.Id,
                t => new object[]
                       {
                           t.screen_name,
                           t.LayerId,
                           t.Name,
                           t.Description,
                           t.Latitude,
                           t.Longitude,
                           t.Tag,
                           t.CommentCount,
                           t.RatingTotal+" ("+t.RatingCount+")",
                           t.ModifiedOn.Value.ToString("dd-MMM-yyyy HH:mm"),
                           t.CreatedOn.Value.ToString("dd-MMM-yyyy HH:mm")
                       })
            {
                page = model.Page,
                total = (int)Math.Ceiling(totalCount / (model.Rows * 1.0)),
                records = totalCount
            };

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult UpdatePoint(string id)
        {
            if (Request["oper"] == "edit")
            {
                PointDataSummary summary = PointDataSummary.SingleOrDefault(p => p.Id == int.Parse(id));
                UpdateModel(summary, new string[] { "LayerId", "Name", "Description", "Latitude", "Longitude", "Tag" });
                summary.ModifiedOn = DateTime.Now;
                summary.Update();
                SearchEngine.Instance.Index(summary.Id);
                return Json(summary);
            }
            else if (Request["oper"] == "del")
            {
                PointDataComment.Delete(p => p.SummaryId == int.Parse(id));
                PointDataSummary.Delete(p => p.Id == int.Parse(id));
                SearchEngine.Instance.RemoveFromIndex(int.Parse(id));
                return Json(true);
            }

            throw new Exception("Bad operation.");
        }

        #endregion

        #region comments

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult GetComments()
        {
            CommentsModel model = new CommentsModel();
            UpdateModel(model, new string[] { "Page", "Rows", "SIdx", "SOrd", "Text", "Summary", "Screen_Name", "_Search" });
            var query = PointDataCommentView.All();

            if (model._Search)
            {
                if (model.Screen_Name.Trim() != "") query = query.Where(p => p.screen_name.Contains(model.Screen_Name));
                if (model.Summary.Trim() != "") query = query.Where(p => p.summary.Contains(model.Summary));
                if (model.Text.Trim() != "") query = query.Where(p => p.Text.Contains(model.Text));
            }

            var totalCount = query.Count();
            query = Order<PointDataCommentView>(query, model.SIdx, model.SOrd).ThenByDescending(p => p.Id);
            var sources = query.Skip((model.Page - 1) * model.Rows).Take(model.Rows);
            var gridData = new jqGridData<PointDataCommentView>(
                sources.ToList(),
                t => t.Id,
                t => new object[]
                       {
                            t.screen_name,
                            t.CreatedById,
                            t.summary,
                            t.Text,
                            t.CreatedOn.Value.ToString("dd-MMM-yyyy HH:mm")
                       })
            {
                page = model.Page,
                total = (int)Math.Ceiling(totalCount / (model.Rows * 1.0)),
                records = totalCount
            };

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult UpdateComment(string id)
        {
            if (Request["oper"] == "del")
            {
                PointDataComment.Delete(p => p.Id == int.Parse(id));
                return Json(true);
            }

            throw new Exception("Bad operation.");
        }

        #endregion

        #region users

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult GetUsers()
        {
            UsersModel model = new UsersModel();
            UpdateModel(model, new string[] { "Page", "Rows", "SIdx", "SOrd", "Screen_Name", "_Search", "UserRole", "UserAccess" });
            OAuthAccount.Find(p => p.Id > 0);

            var query = OAuthAccountView.All().Where(p => p.Id > 0);

            if (model._Search)
            {
                if (model.Screen_Name.Trim()!="") query = query.Where(p => p.screen_name.Contains(model.Screen_Name));
                if (model.UserAccess > -1) query = query.Where(p => p.UserAccess == model.UserAccess);
                if (model.UserRole > -1) query = query.Where(p => p.UserRole==model.UserRole);
            }

            var totalCount = query.Count();

            query = Order<OAuthAccountView>(query, model.SIdx, model.SOrd).ThenByDescending(p => p.Id);

            var users = query.Skip((model.Page - 1) * model.Rows).Take(model.Rows).ToList();
            var gridData = new jqGridData<OAuthAccountView>(
                users,
                t => t.Id,
                t => new object[]
                       {
                           t.Id,
                           t.screen_name,
                           (ODAF.Data.Enums.UserAccess)t.UserAccess,
                           (ODAF.Data.Enums.UserRole)t.UserRole,
                           t.LastAccessedOn.ToString("dd-MMM-yyyy HH:mm"),
                           t.CreatedOn.ToString("dd-MMM-yyyy HH:mm"),
                           t.Summaries,
                           t.Comments
                       })
            {
                page = model.Page,
                total = (int)Math.Ceiling(totalCount / (model.Rows * 1.0)),
                records = totalCount
            };

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Administrator)]
        public ActionResult UpdateUser(int id)
        {
            OAuthAccount user = OAuthAccount.SingleOrDefault(p => p.Id == id);
            UpdateModel(user, new string[] { "UserRole", "UserAccess" });
            user.Update();
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}