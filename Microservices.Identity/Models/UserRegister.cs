using System.ComponentModel.DataAnnotations;

namespace Microservices.Identity.Models;

public class UserRegister
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "The field {0} must have {2} and {1} characters", MinimumLength = 6)]
    public string Password { get; set; }
    
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordCompare { get; set; }
}
