using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace dominikz.shared.Attributes;

public class ListNotEmptyAttribute : ValidationAttribute
{
    public int Max { get; set; } = int.MaxValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable list)
            return new ValidationResult($"Value is not assignable from {nameof(IEnumerable)}");

        var enumerable = list as object[] ?? list.Cast<object>().ToArray();
        if (enumerable.Length == 0)
            return new ValidationResult("List contains no elements");

        if (enumerable.Length > Max)
            return new ValidationResult("List too much elements");

        return null;
    }
}