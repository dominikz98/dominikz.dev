using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class EarningCall
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public EarningCallTime Time { get; set; }
    public DateTime Timestamp { get; set; }
}