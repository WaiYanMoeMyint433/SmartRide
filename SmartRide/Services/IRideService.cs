using SmartRide.Models;
using SmartRide.ViewModels;

namespace SmartRide.Services
{
    public interface IRideService
    {

        Task<RideRequest> GetRequestByIdAsync(int requestId);
        // Customer operations
        Task<RideRequest> BookRideAsync(BookRideViewModel model, int customerId);
        Task<bool> CancelRideAsync(int requestId, int customerId);
        Task<RideRequest> GetActiveRequestAsync(int customerId);
        Task<Ride> GetActiveRideAsync(int customerId);
        Task<List<Ride>> GetRecentRidesAsync(int customerId, int count);
        Task<List<Ride>> GetRideHistoryAsync(int customerId);

        // Driver operations
        Task<List<RideRequest>> GetAvailableRequestsAsync(VehicleType vehicleType);
        Task<bool> AcceptRideRequestAsync(int requestId, int driverId);
        Task RejectRideRequestAsync(int requestId, int driverId);
        Task<Ride> GetCurrentRideAsync(int driverId);
        Task<Ride> StartRideAsync(int rideId, int driverId);
        Task<Ride> CompleteRideAsync(int rideId, int driverId);
        Task<List<Ride>> GetDriverRecentRidesAsync(int driverId, int count);
        Task<List<Ride>> GetDriverRideHistoryAsync(int driverId);
        Task UpdateDriverStatusAsync(int driverId, DriverStatus status);

        // Shared operations
        Task<Ride> GetRideByRequestIdAsync(int requestId);
        Task<decimal> GetTodayEarningsAsync(int driverId);
        Task<int> GetTodayRidesCountAsync(int driverId);
        decimal CalculateFare(string pickup, string dropoff, VehicleType vehicleType);
    }
}