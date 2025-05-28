using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class TrackRideViewModel
    {
        public Ride Ride { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string VehicleInfo { get; set; }
        public string EstimatedArrival { get; set; }
        public string CurrentStatus { get; set; }
    }
}
