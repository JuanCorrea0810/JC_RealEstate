using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.PaymentsDTO_s
{
    public class PostPaymentsDTO : IValidatableObject
    {
        

        public DateTime? Date { get; set; }
        public int Value { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
                if (Value < 0)
                {
                    yield return new ValidationResult("El valor debe ser positivo", new string[] { nameof(Value) });
                }
            
        }
    }
}
