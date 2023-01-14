using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.ViewModels.Auth;

public class LoginVm
{
    [Required]
    [MinLength(5)]
    [MaxLength(55)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}