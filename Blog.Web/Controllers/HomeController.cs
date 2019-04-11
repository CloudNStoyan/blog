using System.Threading.Tasks;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private PostService PostService { get; }

        public HomeController(PostService postService)
        {
            this.PostService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await this.PostService.GetLatestPosts(10);

            return this.View(posts);
        }
    }
}
