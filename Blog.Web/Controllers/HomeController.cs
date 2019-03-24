using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private PostService PostService { get; set; }

        public HomeController(PostService postService)
        {
            this.PostService = postService;
        }

        public IActionResult Index()
        {
            var posts = this.PostService.GetLatestPosts(10);

            return this.View(posts);
        }
    }
}
