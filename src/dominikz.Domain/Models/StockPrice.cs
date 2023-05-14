namespace dominikz.Domain.Models;

public class StockPrice
{
    public int Id { get; set; }
    public int EarningCallId { get; set; }
    public long UtcTimestamp { get; set; }
    public decimal Value { get; set; }
    public EarningCall? EarningCall { get; set; }
}