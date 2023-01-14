using System.ComponentModel.DataAnnotations;

namespace dominikz.Domain.ViewModels.Auth;

public class RegisterVm
{
    [Required] 
    [MinLength(5)] 
    [MaxLength(55)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.EmailAddress)]
    [MaxLength(55)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}