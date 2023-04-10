using System.ComponentModel.DataAnnotations;
using dominikz.Domain.Attributes;
using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class IngredientVm : IHasId
{
    [GuidNotEmpty] public Guid Id { get; set; }
    [Required] [MinLength(2)] public string Name { get; set; } = string.Empty;
    [Required] public decimal Value { get; set; } = 1;
    [Required] public IngredientUnit Unit { get; set; }
}
