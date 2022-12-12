using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.UsersDTO_s
{
    public class ChangePassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 8, ErrorMessage = "La Contraseña debe ser entre {2} y {1} caracteres")]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}
