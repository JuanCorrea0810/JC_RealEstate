namespace RealEstate.DTO_s.BuyContractsDTO_s
{
    public class GetBuyContractsDTo
    {
        public int IdBuyContract { get; set; }
        public int IdEst { get; set; }
        public int IdBuyer { get; set; }
        public DateTime? Date { get; set; }
        public int SalePrice { get; set; }
    }
}
