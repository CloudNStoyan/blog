using System.ComponentModel.DataAnnotations;
using Blog.Web.Infrastructure;

namespace Blog.Web.Areas.Admin.Users
{
    public class FormUserViewModel : ViewModel
    {
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Field Name is required!")]
        public string Username { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field Password is required!")]
        public string Password { get; set; }
    }
}
