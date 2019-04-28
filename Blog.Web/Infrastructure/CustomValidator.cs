using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Blog.Web.Infrastructure
{
    public static class CustomValidator
    {
        public static bool Validate<T>(T model) where T : ViewModel
        {
            var context = new ValidationContext(model, null, null);

            var results = new List<ValidationResult>();

            // ReSharper disable once ArgumentsStyleLiteral
            bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            model.ErrorMessages = results.Select(x => x.ErrorMessage).ToArray();

            return isValid;
        }
    }

    public abstract class ViewModel
    {
        public string[] ErrorMessages { get; set; }
    }
}
