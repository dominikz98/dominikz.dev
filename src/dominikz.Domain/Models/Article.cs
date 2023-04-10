using dominikz.Domain.Enums.Blog;

namespace dominikz.Domain.Models;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string HtmlText { get; set; }
    public DateTime? PublishDate { get; set; }
    public ArticleCategoryEnum Category { get; set; }
    public List<string> Tags { get; set; } = new();

    public Article()
    {
        Title = string.Empty;
        HtmlText = string.Empty;
    }
}
