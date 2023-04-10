using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.Models;

public class Media
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime? PublishDate { get; set; }
    public MediaCategoryEnum Category { get; set; }

    public Media()
    {
        Title = string.Empty;
    }
}
