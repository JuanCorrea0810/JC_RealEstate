using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public partial class Mortgage
    {
        public Mortgage()
        {
            Payments = new HashSet<Payment>();
        }

        public int IdMortgage { get; set; }
        public int? FeesNumber { get; set; }
        public int? FeeValue { get; set; }
        public int? TotalValue { get; set; }
        public int IdEstate { get; set; }
        public string IdUser { get; set; }

        public virtual Estate IdEstateNavigation { get; set; }
        public virtual Aspnetuser IdUserNavigation { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
