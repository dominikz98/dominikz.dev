namespace dominikz.Infrastructure.Clients.SupermarktCheck;

public class ProductVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<ProductPriceVm> Prices { get; set; } = Array.Empty<ProductPriceVm>();
    public IReadOnlyCollection<ProductNutritionVm> NutritionalValues { get; set; } = Array.Empty<ProductNutritionVm>();
}