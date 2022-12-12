using Microsoft.AspNetCore.Identity;
using RealEstate.Validations;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class NewIdentityUser : IdentityUser
    {
        [Required]
        public long Dni { get; set; }
        
        [Required]
        public string FirstName { get; set; } = null!;

        
        public string SecondName { get; set; }

        
        [Required]
        public string FirstSurName { get; set; } = null!;

        
        public string SecondSurName { get; set; } = null!;

        [Required]
        
        public string Country { get; set; } = null!;

        

        public string Address { get; set; } = null!;

        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        public int? Age { get; set; }
    }
}
