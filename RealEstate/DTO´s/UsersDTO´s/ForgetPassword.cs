using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.UsersDTO_s
{
    public class ForgetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
