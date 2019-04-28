using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Infrastructure;
using Blog.Web.Models;
using Blog.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Form
{
    [Area(Roles.Admin)]
    [Authorize]
    public class PostController : Controller
    {
        private PostService PostService { get; }

        public PostController(PostService postService)
        {
            this.PostService = postService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormPostModel formPostModel)
        {
            if (!CustomValidator.Validate(formPostModel))
            {
                return this.View(formPostModel);
            }

            int postId = await this.PostService.CreatePost(formPostModel);

            return this.RedirectToAction("Post", "Data", new {area="", id= postId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await this.PostService.GetPostById(id);

            var formEditModel = new FormEditModel
            {
                Content = post.Content,
                Tags = string.Join(", ",post.Tags),
                Title = post.Title,
                Id = id
            };

            return this.View(formEditModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FormEditModel formEditModel)
        {
            if (!CustomValidator.Validate(formEditModel))
            {
                return this.View(formEditModel);
            }

            string[] tags = formEditModel.Tags?.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var postModel = new PostModel
            {
                Content = formEditModel.Content,
                Id = formEditModel.Id,
                Tags = tags,
                Title = formEditModel.Title
            };

            await this.PostService.UpdatePost(postModel);

            return this.Redirect("/data/post/" + postModel.Id);
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

        public async Task<IActionResult> All()
        {
            var posts = await this.PostService.GetAllPosts();

            return this.View(posts);
        }
    }
}