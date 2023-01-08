using System.ComponentModel.DataAnnotations;

namespace dominikz.shared.Attributes;

public class TimespanNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not TimeSpan timespan)
            return false;

        return timespan != TimeSpan.Zero;
    }
}