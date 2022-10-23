namespace dominikz.kernel.ViewModels;

public class ImdbVM
{
    public string Id { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
    public Rating? Rating { get; set; }
    public string ContentRating { get; set; } = string.Empty;
    public List<string> Genre { get; set; } = new();
    public string Year { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
    public List<string> Actors { get; set; } = new();
    public List<string> Directors { get; set; } = new();
    public List<TopCredit> Top_credits { get; set; } = new();
}

public class TopCredit
{
    public string Name { get; set; } = string.Empty;
    public List<string> Value { get; set; } = new();
}

public class Rating
{
    public int Count { get; set; }
    public double Star { get; set; }
}