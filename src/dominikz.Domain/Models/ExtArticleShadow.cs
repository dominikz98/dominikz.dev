using dominikz.Domain.Enums.Blog;

namespace dominikz.Domain.Models;

public class ExtArticleShadow
{
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public ArticleCategoryEnum Category { get; set; }
    public ArticleSourceEnum Source { get; set; }
}
