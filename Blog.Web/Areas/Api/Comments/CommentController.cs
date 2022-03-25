using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Blog.Web.DAL;
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

        public async Task<IActionResult> Get(int postId, int offset)
        {
            var commentModels = await this.CommentService.GetCommentsWithPostId(postId, offset);

            var commentDtos = commentModels.Select(model => new CommentDto
            {
                AvatarUrl = model.User.AvatarUrl,
                CommentId = model.CommentId,
                Content = model.Content,
                Username = model.User.Name
            });

            return this.Ok(commentDtos);
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

            if (string.IsNullOrWhiteSpace(content.Trim()))
            {
                return this.BadRequest();
            }

            string sanitizedContent = Regex.Replace(content, @"(\n){2,}", Environment.NewLine);

            var commentPoco = new CommentPoco
            {
                Content = sanitizedContent,
                UserId = session.UserAccount.UserId,
                PostId = post.Id,
                ParentId = parentId
            };

            int? commentId = await this.CommentService.CreateComment(commentPoco);

            var resposeCommentDto = new CommentDto
            {
                CommentId = commentId,
                Content = sanitizedContent,
                Username = session.UserAccount.Username,
                AvatarUrl = session.UserAccount.Avatar
            };

            return this.Ok(resposeCommentDto);
        }
    }
}