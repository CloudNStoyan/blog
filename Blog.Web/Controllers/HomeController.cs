using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Posts;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers;

public class HomeController : Controller
{
    private PostService PostService { get; }

    public HomeController(PostService postService)
    {
        this.PostService = postService;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await this.PostService.GetLatestPosts(10);

        return this.View(posts);
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