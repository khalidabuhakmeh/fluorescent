using System.Web.Mvc;
using Fluorescent.Web.Models;

namespace Fluorescent.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new HomeModel());
        }
    }
}
