using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.ViewModels.Auth;

public class RefreshVm
{
    [Required] 
    public string ExpiredToken { get; set; } = string.Empty;
    
    [Required] 
    public string RefreshToken { get; set; } = string.Empty;
}