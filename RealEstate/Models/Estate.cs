namespace RealEstate.Models
{
    public partial class Estate
    {
        public Estate()
        {
            Renters = new HashSet<Renter>();
            Rentingcontracts = new HashSet<Rentingcontract>();
        }

        public int IdEstate { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int Rooms { get; set; }
        public int KmsGround { get; set; }
        public string Alias { get; set; }
        public bool? Rented { get; set; }
        public bool? Sold { get; set; }
        public string IdUser { get; set; }

        public virtual Aspnetuser IdUserNavigation { get; set; }
        public virtual Buycontract Buycontract { get; set; }
        public virtual Buyer Buyer { get; set; }
        public virtual Mortgage Mortgage { get; set; }
        public virtual ICollection<Renter> Renters { get; set; }
        public virtual ICollection<Rentingcontract> Rentingcontracts { get; set; }
    }
}
