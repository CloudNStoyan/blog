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

        public async Task<IActionResult> EditForm(int id, [FromQuery] string alert)
        {
            var post = await this.PostService.GetPostById(id);

            string[] alerts = alert?.Split(',');

            if (alerts != null && alerts.Length > 0)
            {
                for (int i = 0; i < alerts.Length; i++)
                {
                    if (alerts[i] == "invalidTitle")
                    {
                        alerts[i] = "* Title cannot be empty or whitespaces only!";
                    } else if (alerts[i] == "invalidContent")
                    {
                        alerts[i] = "* Content cannot be empty or whitespaces only!";
                    } else if (alerts[i] == "invalidTags")
                    {
                        alerts[i] = "* There must be at least 1 valid tag!";
                    }
                    else
                    {
                        alerts[i] = "";
                    }
                }

                alerts = alerts.Select(x => x).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }

            var editModel = new EditModel
            {
                Alerts = alerts,
                Post = post
            };

            return this.View(editModel);
        }

        public async Task<IActionResult> CreatePost(FormPostModel postModel)
        {
            string[] tags = postModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            string[] errors = this.PostService.ValidatePost(postModel.Title, postModel.Content, tags);

            if (errors.Length > 0)
            {
                return this.RedirectToAction("EditForm", "Form", new { id = postModel, alert = string.Join(",", errors) });
            }

            int postId = await this.PostService.CreatePost(postModel);
            return this.RedirectToAction("Post", "Data", postId);
        }

        public async Task<IActionResult> EditPost(FormEditModel editModel)
        {
            string[] tags = editModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            string[] errors = this.PostService.ValidatePost(editModel.Title, editModel.Content, tags);

            if (errors.Length > 0)
            {
                return this.RedirectToAction("EditForm", "Form", new {id = editModel.Id ,alert = string.Join(",", errors)});
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