using System.ComponentModel.DataAnnotations;

namespace SmartRide.Models
{
    public class Driver : User
    {
        public Driver()
        {
            UserType = UserType.Driver;
            Status = DriverStatus.Offline;
        }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public DriverStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<RideRequest> RideRequests { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
    }
}
