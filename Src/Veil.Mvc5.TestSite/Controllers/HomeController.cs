using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;

namespace Veil.Mvc5.TestSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new
            {
                Message = "Model Message",
                Results = GetResults()
            });
        }

        private static IEnumerable<int> GetResults()
        {
            Thread.Sleep(2000);
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
            yield return 5;
        }
    }
}