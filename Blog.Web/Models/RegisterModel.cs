namespace Blog.Web.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string AvatarUrl { get; set; }
    }
}
