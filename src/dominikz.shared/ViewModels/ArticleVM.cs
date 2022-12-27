using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public abstract class ArticleVm : IViewModel
{
    public Guid Id { get; init; }
    public FileVM? Image { get; init; }
    public string Title { get; init; } = string.Empty;
    public PersonVM? Author { get; init; }
    public DateTime Timestamp { get; init; }
    public ArticleCategoryEnum Category { get; init; }
    public string AltCategories { get; set; } = string.Empty;
    public ArticleSourceEnum Source { get; init; }
}

public class ArticleListVm : ArticleVm
{
    public bool Featured { get; set; }
    public string Path { get; set; } = new("about:blank");
}

public class ArticleDetailVm : ArticleVm
{
    public string? Text { get; set; }
    public List<string> Tags { get; init; } = new();
}
