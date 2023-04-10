using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class FoodListVm : IHasId
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public FoodUnit Unit { get; set; }
    public decimal Value { get; set; }
}