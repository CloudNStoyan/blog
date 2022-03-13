using System.Text.Json.Serialization;

namespace Blog.Web.Areas.Api.Post
{
    public class PostDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("post_id")]
        public int PostId { get; set; }
    }
}
