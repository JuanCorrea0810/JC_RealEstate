namespace RealEstate.Models
{
    public partial class Payment
    {
        public int IdPayments { get; set; }
        public int IdMortgage { get; set; }
        public DateTime? Date { get; set; }
        public int Value { get; set; }

        public virtual Mortgage IdMortgageNavigation { get; set; }
    }
}
