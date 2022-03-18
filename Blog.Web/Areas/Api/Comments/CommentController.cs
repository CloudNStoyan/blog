using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Api.Comments
{
    [Area(AuthenticationAreas.Api)]
    public class CommentController : Controller
    {
        private SessionService SessionService { get; }
        private PostService PostService { get; }
        private CommentService CommentService { get; }

        public CommentController(SessionService sessionService,
            PostService postService, CommentService commentService)
        {
            this.SessionService = sessionService;
            this.PostService = postService;
            this.CommentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string content, int postId, int? parentId)
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

            if (parentId.HasValue)
            {
                var parentComment = await this.CommentService.GetCommentById(parentId.Value);

                if (parentComment == null)
                {
                    return this.BadRequest();
                }
            }

            int? commentId = await this.CommentService.CreateComment(content, session.UserAccount.UserId, post.Id, parentId);

            var resposeCommentDto = new CommentDto
            {
                CommentId = commentId,
                Content = content,
                Username = session.UserAccount.Username
            };

            return this.Ok(resposeCommentDto);
        }
    }
}