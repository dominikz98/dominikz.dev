using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.ViewModels.Cookbook;

public class RecipeStepVm
{
    [Required] [Range(1, int.MaxValue)] public int Order { get; set; }
    [Required] [MinLength(1)] public string Text { get; set; } = string.Empty;
}