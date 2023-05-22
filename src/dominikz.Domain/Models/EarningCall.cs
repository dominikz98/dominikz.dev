using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class EarningCall
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string ISIN { get; set; } = string.Empty;
    public EarningCallTime Time { get; set; }
    public long UtcTimestamp { get; set; }
    public bool? EpsFlag { get; set; }
    public bool? RevenueFlag { get; set; }
    public decimal? Growth { get; set; }
    public decimal? Surprise { get; set; }
}