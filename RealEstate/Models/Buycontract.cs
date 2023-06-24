namespace RealEstate.Models
{
    public partial class Buycontract
    {
        public int IdBuyContract { get; set; }
        public int IdEst { get; set; }
        public int IdBuyer { get; set; }
        public DateTime? Date { get; set; }
        public int SalePrice { get; set; }

        public virtual Buyer IdBuyerNavigation { get; set; }
        public virtual Estate IdEstNavigation { get; set; }
    }
}
