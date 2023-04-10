namespace dominikz.Domain.ViewModels.Media;

public class StreamTokenVm
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset Expiration { get; set; }
}