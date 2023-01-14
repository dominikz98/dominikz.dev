using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.Attributes;

public class GuidNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not Guid id)
            return false;

        return id != Guid.Empty;
    }
}