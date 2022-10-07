namespace dominikz.kernel.ViewModels;

public abstract class ArticleVM
{
    public Guid Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public ArticleCategoryEnum Category { get; set; }
}

public class ArticleListVM : ArticleVM
{
    public bool Featured { get; set; }
    public string ImageUrl { get; set; } = string.Empty; 
    public string AuthorUrl { get; set; } = string.Empty;
    public bool Available { get; set; }
}

public class ArticleDetailVM : ArticleVM
{
    public string? HtmlText { get; set; }
    public List<string> Tags { get; set; } = new();
}

public enum ArticleCategoryEnum
{
    ALL = 0,
    CODING = 1,
    MOVIE = 2,
    PROJECT = 3,
    GAMING = 4
}