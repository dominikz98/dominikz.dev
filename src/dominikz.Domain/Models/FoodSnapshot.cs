namespace dominikz.Domain.Models;

public class FoodSnapshot
{
    public Guid FoodId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Store { get; set; }
    public decimal Price { get; set; }
}