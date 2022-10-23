using dominikz.kernel.Contracts;

namespace dominikz.kernel.Filter;

public class ArticleFilter : IFilter
{
    public string? Text { get; set; }
    public ArticleCategoryEnum Category { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Category != ArticleCategoryEnum.ALL)
            result.Add(new(nameof(Category), Category.ToString()!));

        return result;
    }
}

