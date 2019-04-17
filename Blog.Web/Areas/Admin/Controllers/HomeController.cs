using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area(Utilities.AdminArea)]
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}