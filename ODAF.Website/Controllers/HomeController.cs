using System.Web.Mvc;
using BitlyDotNET.Interfaces;
using BitlyDotNET.Implementations;
using System.Configuration;
using ODAF.Data;
using System.Linq;
using website_mvc.Code;

namespace vancouveropendata.Controllers
{
    public class HomeController : BaseController
    {

        public string BitlyLogin {
            get {
                return CloudSettingsResolver.GetConfigSetting("BitlyLogin") as string;
            }
        }

        public string BitlyAPIKey {
            get {
                return CloudSettingsResolver.GetConfigSetting("BitlyAPIKey") as string;
            }
        }

        public ActionResult Index(long? id, string format)
        {
            ViewData["Title"] = "OpenTurf - the Open Data Explorer";
            if (id != null)
            {
                ViewData["PointDataSummaryId"] = id;
            }
            ViewData["AppName"] = CloudSettingsResolver.GetConfigSetting("AppName");
            return View();
        }

        public ActionResult Credits()
        {
            return View();
        }

        [OutputCache(CacheProfile = "GetLinkForSummary")]
        public ActionResult GetLinkForSummary(long id)
        {
            PointDataSummary summary = PointDataSummary.Find(c => c.Id == id).SingleOrDefault();
            if (summary == null) {
                HttpContext.Response.StatusCode = 404;
                return Json(new { SummaryId = id });
            }

            IBitlyService service = new BitlyService(BitlyLogin, BitlyAPIKey);
            string long_url = this.BuildUrlFromExpression<SummariesController>(c => c.ShowById(id, "html"));
            string short_url = null;
            service.Shorten(long_url, out short_url);
            
            return Json(new { SummaryId = id, long_url = long_url, short_url = short_url });
        }

        [OutputCache(CacheProfile = "GetLinkForComment")]
        public ActionResult GetLinkForComment(long id)
        {
            PointDataComment comment = PointDataComment.Find(c => c.Id == id).SingleOrDefault();
            if (comment == null)
            {
                HttpContext.Response.StatusCode = 404;
                return Json(new { CommentId = id });
            }

            IBitlyService service = new BitlyService(BitlyLogin, BitlyAPIKey);
            string long_url = this.BuildUrlFromExpression<CommentsController>(c => c.Show(id, "html"));

            string short_url = null;
            service.Shorten(long_url, out short_url);

            return Json(new { CommentId = id, long_url = long_url, short_url = short_url });
        }

    }
}