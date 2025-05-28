// Controllers/DriverController.cs
using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.ViewModels;
using SmartRide.Services;

namespace SmartRide.Controllers
{
    public class DriverController : Controller
    {
        private readonly IRideService _rideService;
        private readonly IUserService _userService;

        public DriverController(IRideService rideService, IUserService userService)
        {
            _rideService = rideService;
            _userService = userService;
        }

        // GET: /Driver/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            var driver = await _userService.GetUserByIdAsync(driverId.Value) as Driver;
            if (driver == null)
                return RedirectToAction("Login", "Account");

            var viewModel = new DriverDashboardViewModel
            {
                Driver = driver,
                PendingRequests = await _rideService.GetAvailableRequestsAsync(driver.VehicleType),
                CurrentRide = await _rideService.GetCurrentRideAsync(driverId.Value),
                RecentRides = await _rideService.GetDriverRecentRidesAsync(driverId.Value, 5),
                TodayEarnings = await _rideService.GetTodayEarningsAsync(driverId.Value),
                TodayRides = await _rideService.GetTodayRidesCountAsync(driverId.Value)
            };

            return View(viewModel);
        }

        // POST: /Driver/GoOnline
        [HttpPost]
        public async Task<IActionResult> GoOnline()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                await _rideService.UpdateDriverStatusAsync(driverId.Value, DriverStatus.Online);
                TempData["Success"] = "You are now online and available for rides!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error going online";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: /Driver/GoOffline
        [HttpPost]
        public async Task<IActionResult> GoOffline()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                await _rideService.UpdateDriverStatusAsync(driverId.Value, DriverStatus.Offline);
                TempData["Success"] = "You are now offline";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error going offline";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: /Driver/AcceptRide/{requestId}
        [HttpPost]
        public async Task<IActionResult> AcceptRide(int requestId)
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var success = await _rideService.AcceptRideRequestAsync(requestId, driverId.Value);
                if (success)
                {
                    TempData["Success"] = "Ride accepted! Navigate to pickup location.";
                    return RedirectToAction("CurrentRide");
                }
                else
                {
                    TempData["Error"] = "Unable to accept ride. It may have been taken by another driver.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error accepting ride";
            }

            return RedirectToAction("Dashboard");
        }

        // POST: /Driver/RejectRide/{requestId}
        [HttpPost]
        public async Task<IActionResult> RejectRide(int requestId)
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                await _rideService.RejectRideRequestAsync(requestId, driverId.Value);
                TempData["Info"] = "Ride request rejected";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error rejecting ride";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: /Driver/CurrentRide
        public async Task<IActionResult> CurrentRide()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            var currentRide = await _rideService.GetCurrentRideAsync(driverId.Value);
            if (currentRide == null)
            {
                TempData["Info"] = "No active ride";
                return RedirectToAction("Dashboard");
            }

            return View(currentRide);
        }

        // POST: /Driver/StartRide/{rideId}
        [HttpPost]
        public async Task<IActionResult> StartRide(int rideId)
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var ride = await _rideService.StartRideAsync(rideId, driverId.Value);
                if (ride != null)
                {
                    TempData["Success"] = "Ride started! Navigate to destination.";
                }
                else
                {
                    TempData["Error"] = "Unable to start ride";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error starting ride";
            }

            return RedirectToAction("CurrentRide");
        }

        // POST: /Driver/CompleteRide/{rideId}
        [HttpPost]
        public async Task<IActionResult> CompleteRide(int rideId)
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var ride = await _rideService.CompleteRideAsync(rideId, driverId.Value);
                if (ride != null)
                {
                    TempData["Success"] = $"Ride completed! Fare: ${ride.Fare:F2}";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["Error"] = "Unable to complete ride";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error completing ride";
            }

            return RedirectToAction("CurrentRide");
        }

        // GET: /Driver/RideHistory
        public async Task<IActionResult> RideHistory()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            var rides = await _rideService.GetDriverRideHistoryAsync(driverId.Value);
            return View(rides);
        }

        // GET: /Driver/Earnings
        public async Task<IActionResult> Earnings()
        {
            var driverId = HttpContext.Session.GetInt32("UserId");
            if (!driverId.HasValue)
                return RedirectToAction("Login", "Account");

            var earnings = await _rideService.GetTodayEarningsAsync(driverId.Value);
            ViewBag.TodayEarnings = earnings;
            ViewBag.TodayRides = await _rideService.GetTodayRidesCountAsync(driverId.Value);

            return View();
        }
    }
}