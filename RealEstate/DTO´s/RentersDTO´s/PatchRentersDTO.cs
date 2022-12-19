using RealEstate.Validations;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.RentersDTO_s
{
    public class PatchRentersDTO : IValidatableObject
    {
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string FirsName { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string? SecondName { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string FirstSurName { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string SecondSurName { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [CountryValidation]
        public string Country { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string Address { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        public int? Age { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Número de celular no válido")]
        public long CellPhoneNumber { get; set; }
        
        public bool? Active { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SecondName is not null)
            {
                SecondName = SecondName.ToUpper();
            }

            FirsName = FirsName.ToUpper();
            FirstSurName = FirstSurName.ToUpper();
            SecondSurName = SecondSurName.ToUpper();
            Country = Country.ToUpper();
            yield return ValidationResult.Success;
        }


    }
}
