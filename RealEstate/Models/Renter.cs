namespace RealEstate.Models
{
    public partial class Renter
    {
        public Renter()
        {
            Guarantors = new HashSet<Guarantor>();
            Rentingcontracts = new HashSet<Rentingcontract>();
        }

        public int IdRenter { get; set; }
        public long Dni { get; set; }
        public string FirsName { get; set; }
        public string SecondName { get; set; }
        public string FirstSurName { get; set; }
        public string SecondSurName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public long CellPhoneNumber { get; set; }
        public bool? Active { get; set; }
        public int IdEstate { get; set; }

        public virtual Estate IdEstateNavigation { get; set; }
        public virtual ICollection<Guarantor> Guarantors { get; set; }
        public virtual ICollection<Rentingcontract> Rentingcontracts { get; set; }
    }
}
