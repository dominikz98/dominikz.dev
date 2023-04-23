using dominikz.Domain.Enums.Trades;

namespace dominikz.Domain.ViewModels.Trading;

public class EarningCallVm
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TimeOnly? Release { get; set; } = new();
    public decimal? Growth { get; set; }
    public decimal? Surprise { get; set; }
    public string? ISIN { get; set; } = string.Empty;
    public InformationSource Sources { get; set; }
    public string? LogoUrl { get; set; }
    public string? OnVistaLink { get; set; }
    public string? OnVistaNewsLink { get; set; }
}
