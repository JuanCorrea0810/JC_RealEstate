using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.BuyContractsDTO_s
{
    public class PostBuyContractsDTO : IValidatableObject
    {
        
        public DateTime? Date { get; set; }
        public int SalePrice { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SalePrice < 0)
            {
                yield return new ValidationResult("El valor debe ser positivo", new string[] { nameof(SalePrice) });
            }

        }
    }
}
