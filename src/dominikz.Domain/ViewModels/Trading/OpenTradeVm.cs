using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.ViewModels.Trading;

public class OpenTradeVm
{
    [Required]
    [MinLength(2)]
    public string Symbol { get; set; } = string.Empty;

    public string? Region { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }   
    
    [Required]
    public TimeOnly Timestamp { get; set; }
}