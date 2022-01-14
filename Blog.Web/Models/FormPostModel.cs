using System;
using System.ComponentModel.DataAnnotations;
using Blog.Web.Infrastructure;

namespace Blog.Web.Models
{
    public class FormPostModel : ViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Title cannot be empty or whitespaces only!")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* Content cannot be empty or whitespaces only!")]
        public string Content { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* There must be at least 1 valid tag!")]
        public string Tags { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class FormEditModel : ViewModel
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "* Title cannot be empty or whitespaces only!")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* Content cannot be empty or whitespaces only!")]
        public string Content { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* There must be at least 1 valid tag!")]
        public string Tags { get; set; }

        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}