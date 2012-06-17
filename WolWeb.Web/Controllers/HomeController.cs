using System.Web.Mvc;
using WolWeb.ViewModels.Home;
using WolWeb;

namespace WolWeb.Controllers {

    [AuthorizeRemoteOnly]
    public class HomeController : Controller {

        public ActionResult Index() {
            var model = new IndexViewModel(User.Identity,  Server.MapPath("~/App_Data/Hosts.txt"));
            return View(model);
        }

    }
}
