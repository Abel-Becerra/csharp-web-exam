using System.Web.Mvc;

namespace csharp_web_exam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Products");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Product and Category Management System";
            ViewBag.Description = "This application allows you to manage products and categories through a clean and intuitive interface.";

            return View();
        }
    }
}