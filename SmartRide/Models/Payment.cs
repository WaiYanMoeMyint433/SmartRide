namespace SmartRide.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int RideId { get; set; }
        public virtual Ride Ride { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime PaymentTime { get; set; }
    }
}
