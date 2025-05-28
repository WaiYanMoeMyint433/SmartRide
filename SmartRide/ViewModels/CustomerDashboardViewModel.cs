using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public Customer Customer { get; set; }
        public RideRequest CurrentRequest { get; set; }
        public Ride ActiveRide { get; set; }
        public List<Ride> RecentRides { get; set; } = new();
    }
}