using System.Web.Mvc;

namespace vancouveropendata.Controllers
{
    public class TestController : BaseController
    {
        [Prerequisite(Authenticated = true, Role = ODAF.Data.Enums.UserRole.Moderator)]
        public ActionResult Index()
        {
            // This should just return the View, which should have Test functions
            return View();
        }

    }
}