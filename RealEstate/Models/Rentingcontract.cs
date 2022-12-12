using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public partial class Rentingcontract
    {
        public int IdRentingContract { get; set; }
        public int IdEst { get; set; }
        public int IdRenter { get; set; }
        public DateTime? Date { get; set; }
        public int Value { get; set; }

        public virtual Estate IdEstNavigation { get; set; }
        public virtual Renter IdRenterNavigation { get; set; }
    }
}
