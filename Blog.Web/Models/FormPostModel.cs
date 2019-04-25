using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models
{
    public class FormPostModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Title cannot be empty or whitespaces only!")]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Content cannot be empty or whitespaces only!")]
        public string Content { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* There must be at least 1 valid tag!")]
        public string Tags { get; set; }
    }

    public class FormEditModel
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "* Title cannot be empty or whitespaces only!")]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Content cannot be empty or whitespaces only!")]
        public string Content { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* There must be at least 1 valid tag!")]
        public string Tags { get; set; }
        public int Id { get; set; }
    }

    public class EditModel
    {
        public PostModel Post { get; set; }
        public string[] Alerts { get; set; }
    }
}