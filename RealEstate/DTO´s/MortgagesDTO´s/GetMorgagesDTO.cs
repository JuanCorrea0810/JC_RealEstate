namespace RealEstate.DTO_s.MortgagesDTO_s
{
    public class GetMorgagesDTO
    {
        public int IdMortgage { get; set; }
        public int? FeesNumber { get; set; }
        public int? FeeValue { get; set; }
        public int? TotalValue { get; set; }
        public int IdEstate { get; set; }
    }
}
