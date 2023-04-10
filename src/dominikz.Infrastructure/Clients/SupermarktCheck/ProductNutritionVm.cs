namespace dominikz.Infrastructure.Clients.SupermarktCheck;

public class ProductNutritionVm
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public NutritionUnit Unit { get; set; }
}