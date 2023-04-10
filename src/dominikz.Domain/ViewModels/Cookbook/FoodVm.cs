using System.ComponentModel.DataAnnotations;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class FoodVm
{
    public Guid? Id { get; set; }
    public int SupermarktCheckId { get; set; }
    public decimal Price { get; set; }
    [Required]
    public FoodUnit Unit { get; set; }
    [Required]
    public decimal Value { get; set; }
    [Required] [MinLength(3)] public string Name { get; set; } = string.Empty;
    [Required] public decimal CaloriesInKcal { get; set; }
    [Required] public decimal ProteinInG { get; set; }
    [Required] public decimal CarbohydratesInG { get; set; }
    [Required] public decimal FatInG { get; set; }
    [Required] public decimal DietaryFiberInG { get; set; }
    [Required] public decimal SugarInG { get; set; }
    [Required] public decimal SaltInG { get; set; }
}