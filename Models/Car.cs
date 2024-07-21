using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Car
    {
        public Car()
        {
            Insurances = new HashSet<Insurance>();
            Maintenances = new HashSet<Maintenance>();
            RentalContracts = new HashSet<RentalContract>();
            RentalHistories = new HashSet<RentalHistory>();
        }

        public int CarId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public int CarTypeId { get; set; }
        public int BranchId { get; set; }
        public int ManufacturerId { get; set; }
        public string? Image { get; set; }
        public decimal RentalPrice { get; set; }
        public string Transmission { get; set; } = null!;
        public int Seats { get; set; }
        public string Fuel { get; set; } = null!;
        public string? Description { get; set; }
        public int? Luggage { get; set; }
        public bool? Status { get; set; }

        public virtual Branch Branch { get; set; } = null!;
        public virtual CarType CarType { get; set; } = null!;
        public virtual CarManufacturer Manufacturer { get; set; } = null!;
        public virtual ICollection<Insurance> Insurances { get; set; }
        public virtual ICollection<Maintenance> Maintenances { get; set; }
        public virtual ICollection<RentalContract> RentalContracts { get; set; }
        public virtual ICollection<RentalHistory> RentalHistories { get; set; }
    }
}
