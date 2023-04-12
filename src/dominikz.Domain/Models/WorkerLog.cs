namespace dominikz.Domain.Models;

public class WorkerLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Worker { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Log { get; set; }
}