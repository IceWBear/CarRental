using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Insurance
    {
        public int InsuranceId { get; set; }
        public int CarId { get; set; }
        public string InsuranceCompany { get; set; } = null!;
        public string PolicyNumber { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }

        public virtual Car Car { get; set; } = null!;
    }
}
