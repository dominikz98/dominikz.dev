using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class Media
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime? PublishDate { get; set; }
    public MediaCategoryEnum Category { get; set; }

    public StorageFile? File { get; set; }

    public Media()
    {
        Title = string.Empty;
    }
}
