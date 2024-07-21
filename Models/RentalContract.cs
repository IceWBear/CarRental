using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class RentalContract
    {
        public int ContractId { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;

        public virtual Car Car { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
    }
}
