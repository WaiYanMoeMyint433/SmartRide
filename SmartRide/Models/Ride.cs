namespace SmartRide.Models
{
    public class Ride
    {
        public int RideId { get; set; }

        public int RequestId { get; set; }
        public virtual RideRequest RideRequest { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public RideStatus Status { get; set; }

        public decimal Distance { get; set; }
        public decimal Fare { get; set; }

        // Navigation property
        public virtual Payment Payment { get; set; }
    }
}
