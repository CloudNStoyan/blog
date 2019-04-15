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
            return this.View();
        }

        public async Task<IActionResult> EditForm(int id)
        {
            var post = await this.PostService.GetPostById(id);

            return this.View(post);
        }

        public async Task<IActionResult> CreatePost(FormPostModel postModel)
        {
            int postId = await this.PostService.CreatePost(postModel);
            return this.RedirectToAction("Post", "Data", postId);
        }

        public async Task<IActionResult> EditPost(FormEditModel editModel)
        {
            var postModel = new PostModel
            {
                Content = editModel.Content,
                Id = editModel.Id,
                Tags = editModel.Tags.Split(','),
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