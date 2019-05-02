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
    [Authorize]
    [Area(AuthenticationAreas.Admin)]
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
        public async Task<IActionResult> Create(FormPostModel model)
        {
            if (!CustomValidator.Validate(model))
            {
                return this.View(model);
            }

            int postId = await this.PostService.CreatePost(model);

            return this.RedirectToAction("Post", "Data", new {area="", id= postId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await this.PostService.GetPostById(id);

            var model = new FormEditModel
            {
                Content = post.Content,
                Tags = string.Join(", ",post.Tags),
                Title = post.Title,
                Id = id
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FormEditModel model)
        {
            if (!CustomValidator.Validate(model))
            {
                return this.View(model);
            }

            string[] tags = model.Tags?.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var postModel = new PostModel
            {
                Content = model.Content,
                Id = model.Id,
                Tags = tags,
                Title = model.Title
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