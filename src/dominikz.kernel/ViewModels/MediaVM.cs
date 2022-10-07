namespace dominikz.kernel.ViewModels;

public class MediaVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public MediaCategoryEnum Category { get; set; }
    public List<MediaGenre> Genres { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public enum MediaCategoryEnum
{
    ALL = 0,
    MOVIE = 1,
    SERIES = 2,
    BOOK = 3,
    GAME = 4
}

[Flags]
public enum MediaGenre
{
    ALL = 0,
    SUPERHERO = 1,
    HORROR = 2,
    DRAMA = 4,
    COOP = 8,
    TRASH = 16
}