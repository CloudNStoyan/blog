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

        public IActionResult Post(int id)
        {
            var post = this.PostService.GetPostById(id);

            return this.View(post);
        }
    }
}