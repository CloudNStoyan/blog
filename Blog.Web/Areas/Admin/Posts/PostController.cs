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

        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(FormEditModel model)
        {
            if (model.Id > 0)
            {
                if (!CustomValidator.Validate(model))
                {
                    var invalidEditModel = new CreateOrEditModel()
                    {
                        Header = "Editing Post",
                        Post = model
                    };

                    return this.View(invalidEditModel);
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

                return this.Redirect("/home/post/" + postModel.Id);
            }

            if (!CustomValidator.Validate(model))
            {
                model.Id = 0;
                var invalidCreateModel = new CreateOrEditModel()
                {
                    Header = "Create Post",
                    Post = model
                };

                return this.View(invalidCreateModel);
            }

            var validCreateModel = new FormPostModel()
            {
                Content = model.Content,
                Tags = model.Tags,
                Title = model.Title
            };

            int postId = await this.PostService.CreatePost(validCreateModel);

            return this.RedirectToAction("Post", "Home", new { area = "", id = postId });
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int? id)
        {
            if (id == null)
            {
                var createModel = new CreateOrEditModel
                {
                    Header = "Creating Post",
                    Post = null
                };

                return this.View(createModel);
            }

            int postId = id.Value;

            var post = await this.PostService.GetPostById(postId);

            var editModel = new CreateOrEditModel
            {
                Header = "Editing Post",
                Post = new FormEditModel
                {
                    Content = post.Content,
                    Tags = string.Join(", ", post.Tags),
                    Title = post.Title,
                    Id = postId
                }
            };

            return this.View(editModel);
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