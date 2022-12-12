using RealEstate.Validations;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.GuarantorDTO_s
{
    public class PostGuarantorDTO : IValidatableObject
    {
        public long Dni { get; set; }
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string FirsName { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string? SecondName { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string FirstSurName { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string SecondSurName { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [CountryValidation]
        public string Country { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string Address { get; set; } = null!;

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = null!;

        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        public int? Age { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Número de celular no válido")]
        public long CellPhoneNumber { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Dni == 0)
            {
                yield return new ValidationResult("El campo DNI es requerido", new string[] { nameof(Dni) });
            }

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
