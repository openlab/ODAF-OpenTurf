using System.Web.Mvc;

namespace vancouveropendata.Controllers
{
    public class TestController : BaseController
    {
        public ActionResult Index()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

    }
}