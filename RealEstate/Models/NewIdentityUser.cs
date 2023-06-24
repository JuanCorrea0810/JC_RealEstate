using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class NewIdentityUser : IdentityUser
    {
        
        public long Dni { get; set; }
        
        
        public string FirstName { get; set; } 

        
        public string SecondName { get; set; }

        
        
        public string FirstSurName { get; set; } 

        
        public string SecondSurName { get; set; } 

       
        
        public string Country { get; set; } 

        

        public string Address { get; set; } 

        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        public int? Age { get; set; }
    }
}
