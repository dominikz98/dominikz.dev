using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.Attributes;

public class ListNotEmptyAttribute : ValidationAttribute
{
    public int Max { get; set; } = int.MaxValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable list)
            return new ValidationResult($"{validationContext.DisplayName}: Value is not assignable from {nameof(IEnumerable)}");

        var enumerable = list.Cast<object>().ToList();
        if (enumerable.Count == 0)
            return new ValidationResult($"{validationContext.DisplayName}: List contains no elements");

        if (enumerable.Count > Max)
            return new ValidationResult($"{validationContext.DisplayName}: List contains too much elements");

        return null;
    }
}