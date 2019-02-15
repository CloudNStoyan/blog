using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Post(int id)
        {
            var container = MainContainer.Configure();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<PostService>();

                var post = service.GetPostById(id);

                return View(post);
            }

        }
    }
}