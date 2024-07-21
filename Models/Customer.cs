using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Feedbacks = new HashSet<Feedback>();
            RentalContracts = new HashSet<RentalContract>();
            RentalHistories = new HashSet<RentalHistory>();
            Transactions = new HashSet<Transaction>();
        }

        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Password { get; set; } = null!;
        public decimal Balance { get; set; }
        public int PasswordAttempts { get; set; }
        public DateTime? UnlockTime { get; set; }
        public int Role { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<RentalContract> RentalContracts { get; set; }
        public virtual ICollection<RentalHistory> RentalHistories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
