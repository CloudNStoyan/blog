namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// The login model used for databinding requests
    /// </summary>
    public class LoginDataModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
