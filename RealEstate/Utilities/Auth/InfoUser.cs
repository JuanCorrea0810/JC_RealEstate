using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Auth
{
    public class InfoUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength:50,MinimumLength =8,ErrorMessage ="La Contraseña debe ser entre {1} y {2} caracteres")]
        public string Password { get; set; }
        [Required]
        [Compare("Password",ErrorMessage ="Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } 
    }
}
