using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class DataController : Controller
    {
        public PostService PostService { get; set; }


        public DataController(PostService postService)
        {
            this.PostService = postService;
        }

        public IActionResult Post(int id)
        {
            //var container = ContainerFactory.Create();
            //
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var service = scope.Resolve<PostService>();
            //
            //    var post = service.GetPostById(id);
            //
            //    return View(post);
            //}

            var post = this.PostService.GetPostById(id);
            return this.View(post);


        }
    }
}