using RealEstate.Models.Countries;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Validations
{
    public class CountryValidationAttribute : ValidationAttribute
    {
        

        private readonly Countries countries = new Countries();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Se debe ingresar un país válido");
            }
            value = value.ToString().ToUpper();
            if (!countries.ListCountries.Contains(value))
            {
                return new ValidationResult("El país no es válido", new string[] { nameof(value) });
            }
             return ValidationResult.Success;
        }
    }
}
