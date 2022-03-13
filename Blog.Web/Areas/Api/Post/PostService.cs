using Blog.Web.Models;

namespace Blog.Web.Areas.Api.Post
{
    public class ApiPostService
    {
        public PostDto ConvertPostModelToPostDto(PostModel postModel) => new()
        {
            PostId = postModel.Id,
            Title = postModel.Title
        };
    }
}
