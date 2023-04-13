namespace dominikz.Domain.ViewModels.Movies;

public class StreamTokenVm
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset Expiration { get; set; }
}