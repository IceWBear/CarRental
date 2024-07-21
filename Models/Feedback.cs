using System;
using System.Collections.Generic;

namespace Project_RentACar.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public string? FeedbackContent { get; set; }
        public int Rating { get; set; }
        public DateTime FeedbackDate { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}
