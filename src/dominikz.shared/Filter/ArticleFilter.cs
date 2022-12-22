using dominikz.shared.Contracts;

namespace dominikz.shared.Filter;

public class ArticleFilter : IFilter
{
    public string? Text { get; init; }
    public ArticleSource? Sources { get; init; }
    public ArticleCategoryEnum? Category { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        if (Category != null)
            result.Add(new(nameof(Category), Category.ToString()!));

        if (Sources != null)
            result.Add(new (nameof(Sources), ((int)Sources).ToString()));
            
        return result;
    }
}

