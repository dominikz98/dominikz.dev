namespace dominikz.Api.Models;

public class Author
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Name { get; set; }    

    public List<Article> Articles { get; set; } = new();
    public List<Media> Medias { get; set; } = new();
    public StorageFile? File { get; set; }

    public Author()
    {
        Name = string.Empty;
    }
}
