namespace dominikz.Domain.ViewModels.Trading;

public class TradeVm
{
    public int Id { get; set; }
    public decimal BuyIn { get; set; }
    public DateTime BuyInTimestamp { get; set; }
    public decimal? BuyOut { get; set; }
    public DateTime? BuyOutTimestamp { get; set; }
    public int StockCount { get; set; }
    public decimal? Fee { get; set; }
    public decimal? Tax { get; set; }
}