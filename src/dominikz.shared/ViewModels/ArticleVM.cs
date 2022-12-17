using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public abstract class ArticleVm : IViewModel
{
    public Guid Id { get; set; }
    public FileVM? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public PersonVM? Author { get; set; }
    public DateTime Timestamp { get; set; }
    public ArticleCategoryEnum Category { get; set; }
    public ArticleSource Source { get; set; }
}

public class ArticleListVm : ArticleVm
{
    public bool Featured { get; set; }
    public string Path { get; set; } = new("about:blank");
}

public class ArticleDetailVm : ArticleVm
{
    public string? Text { get; set; }
    public List<string> Tags { get; set; } = new();
}
