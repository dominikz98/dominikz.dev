using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Blog;

namespace dominikz.Domain.Filter;

public class ArticleFilter : IFilter
{
    public string? Text { get; init; }
    public ArticleSourceEnum? Source { get; init; }
    public ArticleCategoryEnum? Category { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        if (Category != null)
            result.Add(new(nameof(Category), Category.ToString()!));

        if (Source != null)
            result.Add(new (nameof(Source), Source.ToString()!));
            
        return result;
    }
}

