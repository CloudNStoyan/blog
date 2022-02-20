using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers;

public class HomeController : Controller
{
    private PostService PostService { get; }
    private SessionService SessionService { get; }

    public HomeController(PostService postService, SessionService sessionService)
    {
        this.PostService = postService;
        this.SessionService = sessionService;
    }

    public async Task<IActionResult> Index(string search, int offset)
    {
        var filter = new PostFilter
        {
            Limit = 10,
            Offset = offset,
            OrderBy = PostFilterOrderBy.UpdatedOn,
            Sort = PostFilterSort.Descending,
            Search = search
        };

        var filteredPostsModel = await this.PostService.GetPosts(filter);

        return this.View(filteredPostsModel);
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

    public async Task<IActionResult> MyPosts()
    {
        var session = this.SessionService.Session;

        if (session?.UserAccount == null)
        {
            return this.RedirectToAction("Index");
        }

        var filter = new PostFilter
        {
            OrderBy = PostFilterOrderBy.UpdatedOn,
            Sort = PostFilterSort.Descending,
            UserId = session.UserAccount.UserId
        };

        var filteredPostsModel = await this.PostService.GetPosts(filter);

        return this.View(filteredPostsModel.Posts);
    }
}