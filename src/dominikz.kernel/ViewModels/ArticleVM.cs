using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public abstract class ArticleVM : IViewModel
{
    public Guid Id { get; set; }
    public FileVM? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public PersonVM? Author { get; set; }
    public DateTime Timestamp { get; set; }
    public ArticleCategoryEnum Category { get; set; }
}

public class ArticleListVM : ArticleVM
{
    public bool Featured { get; set; }
    public string Path { get; set; } = new("about:blank");
}

public class ArticleDetailVM : ArticleVM
{
    public string? Text { get; set; }
    public List<string> Tags { get; set; } = new();
}
