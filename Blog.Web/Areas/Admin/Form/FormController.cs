using System.Threading.Tasks;
using Blog.Web.Models;
using Blog.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Form
{
    [Area("Admin")]
    [Authorize]
    public class FormController : Controller
    {
        private PostService PostService { get; }

        public FormController(PostService postService)
        {
            this.PostService = postService;
        }

        public IActionResult CreateForm()
        {
            return this.View("PostForm");
        }

        public async Task<IActionResult> CreatePost(FormPostModel postModel)
        {
            int postId = await this.PostService.CreatePost(postModel);
            return this.Redirect("/data/post/" + postId);
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await this.PostService.GetAllPosts();

            return this.View(posts);
        }
    }
}