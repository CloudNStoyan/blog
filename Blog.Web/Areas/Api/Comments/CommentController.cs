using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Blog.Web.Areas.Admin.Users;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Api.Comments
{
    [Area(AuthenticationAreas.Api)]
    public class CommentController : Controller
    {
        private UserService UserService { get; }
        private SessionService SessionService { get; }
        private PostService PostService { get; }
        private CommentService CommentService { get; }

        public CommentController(UserService userService, SessionService sessionService,
            PostService postService, CommentService commentService)
        {
            this.UserService = userService;
            this.SessionService = sessionService;
            this.PostService = postService;
            this.CommentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string content, int postId)
        {
            var session = this.SessionService.Session;

            if (!session.IsLoggedIn)
            {
                return this.BadRequest();
            }

            var post = await this.PostService.GetPostById(postId);

            if (post is null)
            {
                return this.BadRequest();
            }

            await this.CommentService.CreateComment(content, session.UserAccount.UserId, post.Id);

            return this.Ok();
        }
    }
}