using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Web.Areas.Api.Post
{
    [Area(AuthenticationAreas.Api)]
    public class PostController : Controller
    {
        private PostService PostService { get; }
        private ApiPostService ApiPostService { get; }

        public PostController(PostService postService, ApiPostService apiPostService)
        {
            this.PostService = postService;
            this.ApiPostService = apiPostService;
        }


        public async Task<IActionResult> Search(string searchTerm)
        {
            var filter = new PostFilter
            {
                Search = searchTerm
            };

            var filteredPosts = await this.PostService.GetPosts(filter);

            if (filteredPosts.Posts.Length == 0)
            {
                return this.Ok();
            }

            var postDtos = filteredPosts.Posts.Select(this.ApiPostService.ConvertPostModelToPostDto);

            return this.Json(postDtos);
        }
    }
}