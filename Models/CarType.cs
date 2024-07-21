using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class CarType
    {
        public CarType()
        {
            Cars = new HashSet<Car>();
        }

        public int CarTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
