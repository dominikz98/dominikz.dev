namespace dominikz.Infrastructure.Extensions;

public static class ListExtensions
{
    public static bool ContainsCleaned<T>(this List<T> source, string compare)
        => source.Select(x => x?.ToString()?.Replace("-", "").ToLower()).Contains(compare.ToLower().Replace("-", ""));
}