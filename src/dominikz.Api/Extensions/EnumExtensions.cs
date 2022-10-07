namespace dominikz.Api.Extensions;

public static class EnumExtensions
{
    public static List<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : struct, Enum
        => Enum.GetValues<TEnum>()
            .Where(x => value.HasFlag(x))
            .ToList();
}
