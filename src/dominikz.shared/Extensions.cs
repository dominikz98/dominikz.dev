namespace dominikz.shared;

public static class Extensions
{
    public static List<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : struct, Enum
        => Enum.GetValues<TEnum>()
            .Where(x => value.HasFlag(x))
            .ToList();

    public static IEnumerable<List<T>> Batch<T>(this List<T> source, int size)
    {
        var result = new List<List<T>>();
        for (int i = 0; i < source.Count; i += size)
            yield return source.GetRange(0, size);

        var batch = new List<T>();
        for (int i = result.Count * size; i < source.Count; i++)
            batch.Add(source[i]);

        if (batch.Count > 0)
            yield return batch;

        yield break;
    }
}
