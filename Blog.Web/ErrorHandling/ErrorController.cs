using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ErrorHandling
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return this.View();
        }

        public IActionResult SomethingWentWrong()
        {
            return this.View();
        }
    }
}