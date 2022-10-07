using dominikz.kernel.ViewModels;

namespace dominikz.Api.Models;

public class Media
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; }
    public int Rating { get; set; }
    public DateTime Timestamp { get; set; }
    public MediaCategoryEnum Category { get; set; }
    public MediaGenre Genres { get; set; } = new();

    public Author? Author { get; set; }
    public StorageFile? File { get; set; }

    public Media()
    {
        Title = string.Empty;
    }
}
