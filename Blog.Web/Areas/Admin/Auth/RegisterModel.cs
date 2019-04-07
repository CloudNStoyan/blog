namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// The register model used for databinding requests
    /// </summary>
    public class RegisterDataModel
    {
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string AvatarUrl { get; set; }
    }
}
