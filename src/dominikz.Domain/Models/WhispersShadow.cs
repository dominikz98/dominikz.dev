namespace dominikz.Domain.Models;

public class WhispersShadow
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Release { get; set; }
}