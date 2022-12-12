using RealEstate.Validations;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.UsersDTO_s
{
    public class PutUsersDTO: IValidatableObject
    {
        [Required]
        public long Dni { get; set; }
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Required]
        public string FirstName { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string SecondName { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Required]
        public string FirstSurName { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string SecondSurName { get; set; } = null!;

        [Required]
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [CountryValidation]
        public string Country { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]

        public string Address { get; set; } = null!;

        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        public int Age { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Número de celular no válido")]
        public  string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SecondName is not null)
            {
                SecondName = SecondName.ToUpper();
            }
            if (SecondSurName is not null)
            {
                SecondSurName = SecondSurName.ToUpper();
            }
            FirstName = FirstName.ToUpper();
            FirstSurName = FirstSurName.ToUpper();           
            Country = Country.ToUpper();
            yield return ValidationResult.Success;
        }
    }
}
