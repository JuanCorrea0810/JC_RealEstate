namespace RealEstate.DTO_s.PaymentsDTO_s
{
    public class GetPaymentsDTO
    {
        public int IdPayments { get; set; }
        public int IdMortgage { get; set; }
        public DateTime? Date { get; set; }
        public int Value { get; set; }
    }
}
