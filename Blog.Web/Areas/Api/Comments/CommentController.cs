using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Blog.Web.Areas.Admin.Users;
using Blog.Web.DAL;
using Blog.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Api.Comments
{
    [Area(AuthenticationAreas.Api)]
    public class CommentController : Controller
    {
        private SessionService SessionService { get; }
        private PostService PostService { get; }
        private CommentService CommentService { get; }
        private UserService UserService { get; }

        public CommentController(SessionService sessionService,
            PostService postService, CommentService commentService, UserService userService)
        {
            this.SessionService = sessionService;
            this.PostService = postService;
            this.CommentService = commentService;
            this.UserService = userService;
        }

        public async Task<IActionResult> Get(int postId, int offset)
        {
            var commentModels = await this.CommentService.GetCommentsWithPostId(postId, offset);

            var commentDtos = commentModels.Select(model => new CommentDto
            {
                AvatarUrl = model.User.AvatarUrl,
                CommentId = model.CommentId,
                Content = model.Content,
                Username = model.User.Name,
                HumanDate = DateUtils.DateTimeToLongAgo(model.CreatedOn),
                Edited = model.Edited
            });

            return this.Ok(commentDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string content, int commentId)
        {
            var session = this.SessionService.Session;

            if (!session.IsLoggedIn)
            {
                return this.BadRequest();
            }

            var commentPoco = await this.CommentService.GetCommentById(commentId);

            if (commentPoco == null || commentPoco.UserId != session.UserAccount.UserId)
            {
                return this.BadRequest();
            }

            string sanitizedContent = Regex.Replace(content, @"(\n){2,}", Environment.NewLine);

            commentPoco.Content = sanitizedContent;

            await this.CommentService.UpdateComment(commentPoco);

            var commentUser = await this.UserService.GetUserById(commentPoco.UserId);

            var resposeCommentDto = new CommentDto
            {
                CommentId = commentPoco.CommentId,
                Content = sanitizedContent,
                Username = commentUser.Name,
                AvatarUrl = commentUser.AvatarUrl,
                HumanDate = DateUtils.DateTimeToLongAgo(commentPoco.CreatedOn)
            };

            return this.Ok(resposeCommentDto);
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

                // We don't want more than 1 level of nesting
                if (parentComment.ParentId.HasValue)
                {
                    return this.BadRequest();
                }
            }

            if (string.IsNullOrWhiteSpace(content.Trim()))
            {
                return this.BadRequest();
            }

            string sanitizedContent = Regex.Replace(content, @"(\n){2,}", Environment.NewLine);

            var now = DateTime.Now;

            var commentPoco = new CommentPoco
            {
                Content = sanitizedContent,
                UserId = session.UserAccount.UserId,
                PostId = post.Id,
                ParentId = parentId,
                CreatedOn = now
            };

            int? commentId = await this.CommentService.CreateComment(commentPoco);

            var resposeCommentDto = new CommentDto
            {
                CommentId = commentId,
                Content = sanitizedContent,
                Username = session.UserAccount.Username,
                AvatarUrl = session.UserAccount.Avatar,
                HumanDate = DateUtils.DateTimeToLongAgo(now)
            };

            return this.Ok(resposeCommentDto);
        }
    }
}