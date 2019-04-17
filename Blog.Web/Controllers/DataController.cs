using System.Threading.Tasks;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class DataController : Controller
    {
        private PostService PostService { get; }
        
        public DataController(PostService postService)
        {
            this.PostService = postService;
        }

        public async Task<IActionResult> Post(int id)
        {
            var post = await this.PostService.GetPostById(id);

            if (post == null)
            {
                return this.RedirectToAction("PageNotFound", "Error");
            }

            return this.View(post);
        }
    }
}