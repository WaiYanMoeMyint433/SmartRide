// Services/RideService.cs
using Microsoft.EntityFrameworkCore;
using SmartRide.Data;
using SmartRide.Models;
using SmartRide.ViewModels;

namespace SmartRide.Services
{
    public class RideService : IRideService
    {
        private readonly AppDbContext _context;

        // Demo locations with distances from city center (in km)
        private readonly Dictionary<string, decimal> _demoLocations = new()
        {
            ["City Center"] = 0,
            ["Airport"] = 15.5m,
            ["University"] = 8.2m,
            ["Shopping Mall"] = 5.7m,
            ["Hospital"] = 12.3m
        };

        public RideService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RideRequest> GetRequestByIdAsync(int requestId)
        {
            return await _context.RideRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        // CUSTOMER OPERATIONS


        public async Task<RideRequest> BookRideAsync(BookRideViewModel model, int customerId)
        {
            var estimatedFare = CalculateFare(model.PickupLocation, model.DropoffLocation, model.VehicleType);

            var request = new RideRequest
            {
                CustomerId = customerId,
                PickupLocation = model.PickupLocation,
                DropoffLocation = model.DropoffLocation,
                VehicleType = model.VehicleType,
                PaymentMethod = model.PaymentMethod,
                Status = RequestStatus.Pending,
                RequestTime = DateTime.Now,
                EstimatedFare = estimatedFare,
                ExpiryTime = DateTime.Now.AddMinutes(10)
            };

            _context.RideRequests.Add(request);
            await _context.SaveChangesAsync();

            // Try to automatically assign a driver
           // await TryAssignDriverAsync(request.RequestId);

            return request;
        }

        private async Task TryAssignDriverAsync(int requestId)
        {
            var request = await _context.RideRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
            var availableDrivers = await GetAvailableDriversAsync(request.VehicleType);

            /*if (availableDrivers.Any())
            {
                // For demo: assign first available driver
                var driver = availableDrivers.First(); 
                await AcceptRideRequestAsync(requestId, driver.UserId);
            }*/
            
            await Task.CompletedTask;
        }

        private async Task<List<Driver>> GetAvailableDriversAsync(VehicleType vehicleType)
        {
            return await _context.Users.OfType<Driver>()
                .Where(d => d.Status == DriverStatus.Online && d.VehicleType == vehicleType)
                .ToListAsync();
        }

        public async Task<bool> CancelRideAsync(int requestId, int customerId)
        {
            var request = await _context.RideRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null || request.CustomerId != customerId)
                return false;

            // Can only cancel if pending or if assigned but not started
            if (request.Status == RequestStatus.Pending)
            {
                request.Status = RequestStatus.Cancelled;
                await _context.SaveChangesAsync();
                return true;
            }

            if (request.Status == RequestStatus.Accepted)
            {
                var ride = await _context.Rides.FirstOrDefaultAsync(r => r.RequestId == requestId);
                if (ride != null && ride.Status == RideStatus.Assigned)
                {
                    // Cancel the ride and free up the driver
                    request.Status = RequestStatus.Cancelled;
                    _context.Rides.Remove(ride);

                    // Update driver status back to online
                    var driver = await _context.Users.OfType<Driver>().FirstOrDefaultAsync(d => d.UserId == ride.DriverId);
                    if (driver != null)
                    {
                        driver.Status = DriverStatus.Online;
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<RideRequest> GetActiveRequestAsync(int customerId)
        {
            return await _context.RideRequests
                .Where(r => r.CustomerId == customerId &&
                           (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Accepted))
                .FirstOrDefaultAsync();
        }

        public async Task<Ride> GetActiveRideAsync(int customerId)
        {
            return await _context.Rides
                .Include(r => r.Driver)
                .Where(r => r.CustomerId == customerId &&
                           (r.Status == RideStatus.Assigned || r.Status == RideStatus.InProgress))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Ride>> GetRecentRidesAsync(int customerId, int count)
        {
            return await _context.Rides
                //.Include(r => r.Driver)
                .Where(r => r.CustomerId == customerId && r.Status == RideStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetRideHistoryAsync(int customerId)
        {
            return await _context.Rides
                .Include(r => r.Driver)
                .Where(r => r.CustomerId == customerId && r.Status == RideStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .ToListAsync();
        }

        // DRIVER OPERATIONS

        public async Task<List<RideRequest>> GetAvailableRequestsAsync(VehicleType vehicleType)
        {
            return await _context.RideRequests
                .Include(r => r.Customer)
                .Where(r => r.Status == RequestStatus.Pending &&
                           r.VehicleType == vehicleType &&
                           r.ExpiryTime > DateTime.Now)
                .OrderBy(r => r.RequestTime)
                .ToListAsync();
        }

        public async Task<bool> AcceptRideRequestAsync(int requestId, int driverId)
        {
            var request = await _context.RideRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
            var driver = await _context.Users.OfType<Driver>().FirstOrDefaultAsync(d => d.UserId == driverId);

            if (request == null || driver == null ||
                request.Status != RequestStatus.Pending ||
                driver.Status != DriverStatus.Online)
                return false;

            // Update request
            request.DriverId = driverId;
            request.Status = RequestStatus.Accepted;

            // Create ride
            var ride = new Ride
            {
                RequestId = requestId,
                CustomerId = request.CustomerId,
                DriverId = driverId,
                PickupLocation = request.PickupLocation,
                DropoffLocation = request.DropoffLocation,
                Status = RideStatus.Assigned,
                Distance = Math.Abs(_demoLocations[request.PickupLocation] - _demoLocations[request.DropoffLocation]),
                Fare = request.EstimatedFare
            };

            _context.Rides.Add(ride);

            // Update driver status
            driver.Status = DriverStatus.Busy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RejectRideRequestAsync(int requestId, int driverId)
        {
            // For demo purposes, we just log the rejection
            // In a real app, you might track rejections
            await Task.CompletedTask;
        }

        public async Task<Ride> GetCurrentRideAsync(int driverId)
        {
            return await _context.Rides
                .Include(r => r.Customer)
                .Where(r => r.DriverId == driverId &&
                           (r.Status == RideStatus.Assigned || r.Status == RideStatus.InProgress))
                .FirstOrDefaultAsync();
        }

        public async Task<Ride> StartRideAsync(int rideId, int driverId)
        {
            var ride = await _context.Rides.FirstOrDefaultAsync(r => r.RideId == rideId);

            if (ride == null || ride.DriverId != driverId || ride.Status != RideStatus.Assigned)
                return null;

            ride.Status = RideStatus.InProgress;
            ride.StartTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return ride;
        }

        public async Task<Ride> CompleteRideAsync(int rideId, int driverId)
        {
            var ride = await _context.Rides
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.RideId == rideId);

            if (ride == null || ride.DriverId != driverId || ride.Status != RideStatus.InProgress)
                return null;

            ride.Status = RideStatus.Completed;
            ride.EndTime = DateTime.Now;

            // Update driver status back to online
            var driver = ride.Driver as Driver;
            if (driver != null)
            {
                driver.Status = DriverStatus.Online;
            }

            await _context.SaveChangesAsync();
            return ride;
        }

        public async Task<List<Ride>> GetDriverRecentRidesAsync(int driverId, int count)
        {
            return await _context.Rides
                .Include(r => r.Customer)
                .Where(r => r.DriverId == driverId && r.Status == RideStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Ride>> GetDriverRideHistoryAsync(int driverId)
        {
            return await _context.Rides
                .Include(r => r.Customer)
                .Where(r => r.DriverId == driverId && r.Status == RideStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .ToListAsync();
        }

        public async Task UpdateDriverStatusAsync(int driverId, DriverStatus status)
        {
            var driver = await _context.Users.OfType<Driver>().FirstOrDefaultAsync(d => d.UserId == driverId);
            if (driver != null)
            {
                driver.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // SHARED OPERATIONS

        public async Task<Ride> GetRideByRequestIdAsync(int requestId)
        {
            return await _context.Rides
                .Include(r => r.Customer)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public async Task<decimal> GetTodayEarningsAsync(int driverId)
        {
            var today = DateTime.Today;
            return await _context.Rides
                .Where(r => r.DriverId == driverId &&
                           r.Status == RideStatus.Completed &&
                           r.EndTime.HasValue && r.EndTime.Value.Date == today)
                .SumAsync(r => r.Fare);
        }

        public async Task<int> GetTodayRidesCountAsync(int driverId)
        {
            var today = DateTime.Today;
            return await _context.Rides
                .CountAsync(r => r.DriverId == driverId &&
                               r.Status == RideStatus.Completed &&
                               r.EndTime.HasValue && r.EndTime.Value.Date == today);
        }

        public decimal CalculateFare(string pickup, string dropoff, VehicleType vehicleType)
        {
            var distance = Math.Abs(_demoLocations[pickup] - _demoLocations[dropoff]);
            var baseRate = vehicleType == VehicleType.Car ? 2.5m : 1.8m; // per km
            return distance * baseRate + 3.0m; // +3 base fare
        }
    }
}