namespace dominikz.Domain.Models;

public class StockPrice
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Value { get; set; }
}