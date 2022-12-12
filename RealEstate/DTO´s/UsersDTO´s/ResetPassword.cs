using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.UsersDTO_s
{
    public class ResetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 8, ErrorMessage = "La Contraseña debe ser entre {1} y {2} caracteres")]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
        //public string Code { get; set; }    
    }
}
