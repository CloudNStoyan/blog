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

        [HttpGet]
        public IActionResult Create()
        {
            this.ViewData.Add("actionName", nameof(this.Create));
            return this.View("CreateOrEdit");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pocoPost = await this.PostService.GetPostById(id);
            var model = new FormEditModel
            {
                Title = pocoPost.Title,
                Content = pocoPost.Content,
                Id = pocoPost.Id,
                Tags = string.Join(',' ,pocoPost.Tags)
            };

            this.ViewData.Add("actionName", nameof(this.Edit));
            return this.View("CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormPostModel inputModel)
        {
            var model = new FormEditModel
            {
                Content = inputModel.Content,
                Tags = inputModel.Tags,
                Title = inputModel.Title
            };

            if (!CustomValidator.Validate(model))
            {
                this.ViewData.Add("actionName", nameof(this.Create));
                return this.View("CreateOrEdit", model);
            }

            int postId = await this.PostService.CreatePost(inputModel);

            return this.RedirectToAction("Post", "Home", new { area = "", id = postId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FormEditModel model)
        {
            if (!CustomValidator.Validate(model))
            {
                this.ViewData.Add("actionName", nameof(this.Edit));
                return this.View("CreateOrEdit", model);
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

            return this.RedirectToAction("Post", "Home", new { area = "", id = model.Id });
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