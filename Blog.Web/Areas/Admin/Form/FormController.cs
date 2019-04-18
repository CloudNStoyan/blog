using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Models;
using Blog.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Form
{
    [Area(Roles.Admin)]
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
            return this.View();
        }

        public async Task<IActionResult> EditForm(int id)
        {
            var post = await this.PostService.GetPostById(id);

            return this.View(post);
        }

        public async Task<IActionResult> CreatePost(FormPostModel postModel)
        {
            string[] tags = postModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (!this.PostService.ValidatePost(postModel.Title, postModel.Content, tags))
            {
                return this.RedirectToAction("SomethingWentWrong", "Error", new { area = "" });
            }

            int postId = await this.PostService.CreatePost(postModel);
            return this.RedirectToAction("Post", "Data", postId);
        }

        public async Task<IActionResult> EditPost(FormEditModel editModel)
        {
            string[] tags = editModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (!this.PostService.ValidatePost(editModel.Title, editModel.Content, tags))
            {
                return this.RedirectToAction("SomethingWentWrong", "Error", new { area = "" });
            }

            var postModel = new PostModel
            {
                Content = editModel.Content,
                Id = editModel.Id,
                Tags = tags,
                Title = editModel.Title
            };

            await this.PostService.UpdatePost(postModel);

            return this.Redirect("/data/post/" + postModel.Id);
        }

        public async Task<IActionResult> DeletePostForm(int id)
        {
            var post = await this.PostService.GetPostById(id);

            return this.View(post);
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            await this.PostService.DeletePost(id);

            return this.RedirectToAction("Posts", "Form");
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await this.PostService.GetAllPosts();

            return this.View(posts);
        }
    }
}