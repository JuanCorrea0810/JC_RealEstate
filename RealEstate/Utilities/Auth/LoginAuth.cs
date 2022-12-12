using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Auth
{
    public class LoginAuth
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }    
    }
}
