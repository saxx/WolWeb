using System.Web.Mvc;
using WolWeb.ViewModel;

namespace WolWeb.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {
            var model = new PreconfiguredHostsViewModel(Server.MapPath("~/App_Data/Hosts.txt"));
            return View(model);
        }

    }
}
