using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class RentalHistory
    {
        public int HistoryId { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Note { get; set; }

        public virtual Car Car { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
    }
}
