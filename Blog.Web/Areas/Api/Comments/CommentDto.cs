using System;
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

        [JsonPropertyName("human_date")]
        public string HumanDate { get; set; }

        [JsonPropertyName("edited")]
        public bool Edited { get; set; }
    }
}
