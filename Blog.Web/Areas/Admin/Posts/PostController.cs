using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Infrastructure;
using Blog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Posts
{
    [Authorize]
    [Area(AuthenticationAreas.Admin)]
    public class PostController : Controller
    {
        private PostService PostService { get; }

        public PostController(PostService postService)
        {
            this.PostService = postService;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await this.PostService.GetPostById(id);

            return this.View(post);
        }

        public async Task<IActionResult> TakeDelete(int id)
        {
            await this.PostService.DeletePost(id);

            return this.RedirectToAction("All", "Post");
        }

        public IActionResult Create()
        {
            return this.View("CreateOrEdit");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var posts = await this.PostService.GetAllPosts();

            return this.View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> All(string searchTerms)
        {
            var posts = await this.PostService.GetAllPostsWithTerms(searchTerms);

            return this.View(posts);
        }
    }
}