﻿using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.MortgagesDTO_s
{
    public class PatchMortgagesDTO : IValidatableObject
    {
        public int? FeesNumber { get; set; }
        public int? FeeValue { get; set; }
        public int? TotalValue { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            
                if (FeesNumber < 0)
                {
                    yield return new ValidationResult("El valor debe ser positivo", new string[] { nameof(FeesNumber) });
                }
            
            
            
                if (FeeValue < 0)
                {
                    yield return new ValidationResult("El valor debe ser positivo", new string[] { nameof(FeeValue) });
                }
            
            
                if (TotalValue < 0)
                {
                    yield return new ValidationResult("El valor debe ser positivo", new string[] { nameof(TotalValue) });
                }
            
        }
    }
}
