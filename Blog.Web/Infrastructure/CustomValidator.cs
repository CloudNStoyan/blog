using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Infrastructure
{
    public static class CustomValidator
    {
        public static bool Validate<T>(T model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(model, context, results, true);
        }
    }
}
