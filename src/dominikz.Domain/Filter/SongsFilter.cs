using dominikz.Domain.Contracts;

namespace dominikz.Domain.Filter;

public class SongsFilter : IFilter
{
    public string? Text { get; init; }
    
    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        return result;
    }
}