using dominikz.Domain.Enums.Trades;

namespace dominikz.Domain.Models;

public class EarningCall
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TimeOnly? Release { get; set; } = new();
    public decimal? Growth { get; set; }
    public decimal? Surprise { get; set; }
    public string ISIN { get; set; } = string.Empty;
    public string? AktienFinderLogoLink { get; set; }
    public string? OnVistaLink { get; set; }
    public string? OnVistaNewsLink { get; set; }
    public InformationSource Sources { get; set; }
}