namespace dominikz.Domain.ViewModels.Trading;

public class TradeDetailVm
{
    public int Id { get; set; }
    public string ISIN { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Timestamp { get; set; }
    public decimal? BuyIn { get; set; }
    public decimal? BuyOut { get; set; }
    public decimal? Tax { get; set; }
    public decimal? Fee { get; set; }
}