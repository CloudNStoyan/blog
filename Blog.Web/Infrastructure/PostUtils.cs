namespace Blog.Web.Infrastructure
{
    public static class PostUtils
    {
        public static string GetExcerp(string content, int maxLength) => content.Length > maxLength ? $"{content[..maxLength]}.." : content;
    }
}
