using System.ComponentModel.DataAnnotations;

namespace dominikz.shared.Attributes;

public class GuidNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not Guid id)
            return false;

        return id != Guid.Empty;
    }
}