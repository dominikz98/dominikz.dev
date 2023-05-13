using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class EarningCall
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string ISIN { get; set; } = string.Empty;
    public bool LogoAvailable { get; set; }
    public EarningCallTime Time { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal EpsActual { get; set; }
    public decimal EpsEstimate { get; set; }
    public long RevenueActual { get; set; }
    public long RevenueEstimate { get; set; }
    public decimal Q1 { get; set; }
    public decimal Q2 { get; set; }
    public decimal Q3 { get; set; }
    public decimal Q4 { get; set; }
}