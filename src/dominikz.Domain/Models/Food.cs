using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.Models;

public class Food
{
    public Guid Id { get; set; }
    public int? SupermarktCheckId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FoodUnit Unit { get; set; }
    public decimal Value { get; set; }
    public decimal CaloriesInKcal { get; set; }
    public decimal ProteinInG { get; set; }
    public decimal CarbohydratesInG { get; set; }
    public decimal FatInG { get; set; }
    public decimal DietaryFiberInG { get; set; }
    public decimal SugarInG { get; set; }
    public decimal SaltInG { get; set; }
    
    public ICollection<FoodSnapshot> Snapshots { get; set; } = new List<FoodSnapshot>();
}