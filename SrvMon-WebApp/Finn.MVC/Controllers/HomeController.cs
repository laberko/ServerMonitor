using System.Web.Mvc;

namespace Finn.MVC.Controllers
{
	[RequireHttps]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
		public ActionResult Download()
		{
			return View();
		}
		public ActionResult Contact()
		{
			return View();
		}
	}
}