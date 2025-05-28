using System.ComponentModel.DataAnnotations;

namespace SmartRide.Models
{
    public class RideRequest
    {
        public int RequestId { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        [Required]
        public string PickupLocation { get; set; }

        [Required]
        public string DropoffLocation { get; set; }

        public VehicleType VehicleType { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime RequestTime { get; set; }

        public decimal EstimatedFare { get; set; }

        public DateTime ExpiryTime { get; set; }

        // Navigation property
        public virtual Ride Ride { get; set; }
    }
}
