using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class CarManufacturer
    {
        public CarManufacturer()
        {
            Cars = new HashSet<Car>();
        }

        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; } = null!;
        public string Country { get; set; } = null!;

        public virtual ICollection<Car> Cars { get; set; }
    }
}
