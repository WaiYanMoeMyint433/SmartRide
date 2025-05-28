using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class DriverDashboardViewModel
    {
        public Driver Driver { get; set; }
        public List<RideRequest> PendingRequests { get; set; } = new();
        public Ride CurrentRide { get; set; }
        public List<Ride> RecentRides { get; set; } = new();
        public decimal TodayEarnings { get; set; }
        public int TodayRides { get; set; }
    }
}
