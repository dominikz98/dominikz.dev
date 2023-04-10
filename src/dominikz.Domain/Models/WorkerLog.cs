namespace dominikz.Domain.Models;

public class WorkerLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Success { get; set; }
    public string? Log { get; set; }
}