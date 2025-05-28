using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class RideHistoryViewModel
    {
        public int RideId { get; set; }
        public DateTime RideDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public string DriverName { get; set; }
        public string VehicleType { get; set; }
        public decimal Fare { get; set; }
        public RideStatus Status { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Distance { get; set; }
    }
}