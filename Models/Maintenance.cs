using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Maintenance
    {
        public int MaintenanceId { get; set; }
        public int CarId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }

        public virtual Car Car { get; set; } = null!;
    }
}
