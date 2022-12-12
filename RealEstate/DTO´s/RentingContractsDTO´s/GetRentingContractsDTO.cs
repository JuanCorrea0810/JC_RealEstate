namespace RealEstate.DTO_s.RentingContractsDTO_s
{
    public class GetRentingContractsDTO
    {
        public int IdRentingContract { get; set; }
        public int IdEst { get; set; }
        public int IdRenter { get; set; }
        public DateTime? Date { get; set; }
        public int Value { get; set; }
    }
}
