using System.Web.Mvc;

namespace Veil.Mvc5.TestSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new
            {
                Message = "Model Message"
            });
        }
    }
}