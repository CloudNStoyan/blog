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

        public IActionResult CreateForm([FromQuery] string alert, [FromQuery] string title, [FromQuery] string content,
            [FromQuery] string tags)
        {
            string[] alerts = alert?.Split(',');

            if (alerts != null && alerts.Length > 0)
            {
                for (int i = 0; i < alerts.Length; i++)
                {
                    if (alerts[i] == "invalidTitle")
                    {
                        alerts[i] = "* Title cannot be empty or whitespaces only!";
                    }
                    else if (alerts[i] == "invalidContent")
                    {
                        alerts[i] = "* Content cannot be empty or whitespaces only!";
                    }
                    else if (alerts[i] == "invalidTags")
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
                Post = new PostModel
                {
                    Content = content,
                    Tags = !string.IsNullOrWhiteSpace(tags) ? tags.Split(',') : new string[] {""},
                    Title = title
                }

            };

            return this.View(editModel);
        }

        public async Task<IActionResult> EditForm(int id, [FromBody]EditModel returnedEditModel)
        {
            var post = await this.PostService.GetPostById(id);

            string[] alerts = returnedEditModel.Alerts;

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

            if (returnedEditModel.Post != null)
            {
                editModel = new EditModel
                {
                    Alerts = alerts,
                    Post = returnedEditModel.Post
                };
            }

            return this.View(editModel);
        }

        public async Task<IActionResult> CreatePost(FormPostModel postModel)
        {
            string[] tags = postModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            string[] errors = this.PostService.ValidatePost(postModel.Title, postModel.Content, tags);

            if (errors.Length > 0)
            {
                var routeObject = new { alert = string.Join(",", errors), title = postModel.Title, content = postModel.Content, tags = postModel.Tags ?? "" };
                return this.RedirectToAction("CreateForm", "Form", routeObject);
            }

            int postId = await this.PostService.CreatePost(postModel);
            return this.RedirectToAction("Post", "Data", postId);
        }

        public async Task<IActionResult> EditPost(FormEditModel editModel)
        {
            string[] tags = editModel.Tags?.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            string[] errors = this.PostService.ValidatePost(editModel.Title, editModel.Content, tags);

            var postModel = new PostModel
            {
                Content = editModel.Content,
                Id = editModel.Id,
                Tags = tags,
                Title = editModel.Title
            };

            if (errors.Length > 0)
            {
                var routeObject = new { id = editModel.Id, returnedEditModel = new EditModel
                {
                    Alerts = errors,
                    Post = postModel
                }};
                return this.RedirectToAction("EditForm", "Form", routeObject);
            }

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