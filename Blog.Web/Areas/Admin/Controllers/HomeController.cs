using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var a = this.User.Identity.IsAuthenticated;
            var b = this.User.Identity.Name;
            var c = this.User.Identity.AuthenticationType;

            return this.View();
        }
    }
}