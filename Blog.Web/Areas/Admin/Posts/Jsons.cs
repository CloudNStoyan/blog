using Newtonsoft.Json;

namespace Blog.Web.Areas.Admin.Posts;

public class PostJson
{
    [JsonProperty("post_id")]
    public int PostId { get; set; }
}

public class UserJson
{
    [JsonProperty("avatar")]
    public string Avatar { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }
}