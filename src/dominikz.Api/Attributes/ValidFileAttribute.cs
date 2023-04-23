using System.ComponentModel.DataAnnotations;

namespace dominikz.Api.Attributes;

public class ValidFileAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
            return ValidateFile(file);

        else if (value is IEnumerable<IFormFile> files)
        {
            foreach (var item in files)
            {
                var error = ValidateFile(item);
                if (error != null)
                    return error;
            }

            return null;
        }

        else
            return new ValidationResult($"Value is not assignable from {nameof(IFormFile)}");
    }

    private ValidationResult? ValidateFile(IFormFile file)
    {
        if (file.ContentType == string.Empty)
            return new ValidationResult("Invalid content-type");

        if (file.Length == 0)
            return new ValidationResult("Invalid content-length");

        return null;
    }
}