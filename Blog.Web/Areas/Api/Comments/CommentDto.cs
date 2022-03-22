using System.Text.Json.Serialization;

namespace Blog.Web.Areas.Api.Comments
{
    public class CommentDto
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("comment_id")]
        public int? CommentId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
