namespace dominikz.Domain.Models;

public class EarningCall
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Hour { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ISIN { get; set; }
    public DateTime Updated { get; set; }

    public decimal? EpsActual { get; set; }
    public decimal? EpsEstimate { get; set; }
    public long? RevenueActual { get; set; }
    public long? RevenueEstimate { get; set; }
    public decimal? Surprise { get; set; }
    public decimal? SurprisePercent { get; set; }
}