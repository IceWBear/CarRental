using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Cars = new HashSet<Car>();
        }

        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Car> Cars { get; set; }
    }
}
