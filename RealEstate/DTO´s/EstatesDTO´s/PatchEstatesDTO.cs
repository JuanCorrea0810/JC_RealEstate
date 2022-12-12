using RealEstate.Validations;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.EstatesDTO_s
{
    public class PatchEstatesDTO: IValidatableObject

    {
        
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string Address { get; set; } = null!;
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [CountryValidation]
        public string Country { get; set; } = null!;
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string City { get; set; } = null!;

        public int Rooms { get; set; }
        public int KmsGround { get; set; }
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        public string? Alias { get; set; }
        public bool? Rented { get; set; }
        public bool? Sold { get; set; }

      
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Rooms <= 0)
            {
                yield return new ValidationResult("El número de habitaciones no es válido", new string[] { nameof(Rooms) });
            }
            if (KmsGround <= 0)
            {
                yield return new ValidationResult("El número de terreno no es válido", new string[] { nameof(KmsGround) });
            }
            Country = Country.ToUpper();
            City = City.ToUpper();
            yield return ValidationResult.Success;

        }
    }
}
