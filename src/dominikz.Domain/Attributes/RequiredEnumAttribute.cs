using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.Attributes;

public class RequiredEnumAttribute<T> : RequiredAttribute where T : Enum
{
    public T[] Blacklist { get; set; } = Array.Empty<T>();

    public override bool IsValid(object? value)
    {
        if (value is null)
            return false;
        
        if (value is not T parsed)
            return false;

        var isExcluded = Blacklist.Contains(parsed);
        if (isExcluded)
            return false;

        return true;
    }
}