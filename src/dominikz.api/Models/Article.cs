using dominikz.shared.Contracts;

namespace dominikz.api.Models;

public class Article
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; }
    public string MdText { get; set; }
    public DateTime Timestamp { get; set; }
    public ArticleCategoryEnum Category { get; set; }
    public List<string> Tags { get; set; } = new();

    public Person? Author { get; set; }
    public StorageFile? File { get; set; }

    public Article()
    {
        Title = string.Empty;
        MdText = string.Empty;
    }
}
