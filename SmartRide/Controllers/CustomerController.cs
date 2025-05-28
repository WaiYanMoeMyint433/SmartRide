// Controllers/CustomerController.cs
using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.ViewModels;
using SmartRide.Services;

namespace SmartRide.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IRideService _rideService;
        private readonly IUserService _userService;

        public CustomerController(IRideService rideService, IUserService userService)
        {
            _rideService = rideService;
            _userService = userService;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Debug: Check session data
                var customerId = HttpContext.Session.GetInt32("UserId");
                var userName = HttpContext.Session.GetString("UserName");
                var userType = HttpContext.Session.GetString("UserType");

                ViewBag.Debug = $"Session - CustomerId: {customerId}, UserName: {userName}, UserType: {userType}";

                if (!customerId.HasValue)
                {
                    ViewBag.Debug += " | Session UserId is NULL - redirecting to login";
                    return RedirectToAction("Login", "Account");
                }

                // Debug: Try to get user
                ViewBag.Debug += $" | Attempting to get user with ID: {customerId.Value}";

                var user = await _userService.GetUserByIdAsync(customerId.Value);
                ViewBag.Debug += $" | User found: {(user != null ? "YES" : "NO")}";

                if (user != null)
                {
                    ViewBag.Debug += $" | User details: Name={user.Name}, Email={user.Email}, Type={user.UserType}";
                }

                var customer = user as Customer;
                ViewBag.Debug += $" | Cast to Customer: {(customer != null ? "SUCCESS" : "FAILED")}";

                if (customer == null)
                {
                    ViewBag.Debug += " | Customer is null - creating test model";

                    // Create a test model to see if view renders
                    var testModel = new CustomerDashboardViewModel
                    {
                        Customer = new Customer
                        {
                            UserId = customerId.Value,
                            Name = "Test User",
                            Email = "test@test.com",
                            Phone = "123-456-7890",
                            UserType = UserType.Customer
                        },
                        CurrentRequest = null,
                        ActiveRide = null,
                        RecentRides = new List<Ride>()
                    };

                    return View(testModel);
                }

                // Try to create real model
                var viewModel = new CustomerDashboardViewModel
                {
                    Customer = customer,
                    CurrentRequest = null, // Skip for now
                    ActiveRide = null,     // Skip for now  
                    RecentRides = new List<Ride>() // Empty for now
                };

                ViewBag.Debug += " | Model created successfully";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Debug = $"EXCEPTION: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;

                // Return a minimal model to see the error
                var errorModel = new CustomerDashboardViewModel
                {
                    Customer = new Customer { Name = "Error User", Email = "error@test.com" },
                    CurrentRequest = null,
                    ActiveRide = null,
                    RecentRides = new List<Ride>()
                };

                return View(errorModel);
            }
        }



        // GET: /Customer/BookRide
        [HttpGet]
        public IActionResult BookRide()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Account");

            return View(new BookRideViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookRide(BookRideViewModel model)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            if (model.PickupLocation == model.DropoffLocation)
            {
                ModelState.AddModelError("", "Pickup and drop-off locations cannot be the same");
                return View(model);
            }

            try
            {
                // DEBUG: Log what we're trying to save
                ViewBag.Debug = $"CustomerId: {customerId.Value}, Pickup: {model.PickupLocation}, Dropoff: {model.DropoffLocation}, Vehicle: {model.VehicleType}, Payment: {model.PaymentMethod}";

                // Calculate estimated fare
                model.EstimatedFare = _rideService.CalculateFare(
                    model.PickupLocation,
                    model.DropoffLocation,
                    model.VehicleType);

                ViewBag.Debug += $", Fare: {model.EstimatedFare}";

                var request = await _rideService.BookRideAsync(model, customerId.Value);

                TempData["Success"] = "Ride booked successfully! Searching for available drivers...";
                return RedirectToAction("TrackRide", new { requestId = request.RequestId });
            }
            catch (Exception ex)
            {
                // Show detailed error information
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                var fullError = $"Error: {ex.Message} | Inner: {innerException} | Stack: {ex.StackTrace}";

                ModelState.AddModelError("", $"Database Error: {ex.Message}");
                ViewBag.DetailedError = fullError;

                return View(model);
            }
        }


        // GET: /Customer/TrackRide/{requestId}
        public async Task<IActionResult> TrackRide(int requestId)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                // First, get the RideRequest to ensure it exists and belongs to this customer
                var request = await _rideService.GetRequestByIdAsync(requestId);
                if (request == null || request.CustomerId != customerId.Value)
                {
                    TempData["Error"] = "Ride request not found";
                    return RedirectToAction("Dashboard");
                }

                // Check if ride has been created (driver accepted)
                var ride = await _rideService.GetRideByRequestIdAsync(requestId);

                if (ride != null)
                {
                    // Driver has accepted - show ride tracking
                    var viewModel = new TrackRideViewModel
                    {
                        Ride = ride,
                        DriverName = ride.Driver?.Name ?? "Driver Assigned",
                        DriverPhone = ride.Driver?.Phone ?? "N/A",
                        VehicleInfo = ride.Driver != null ?
                            $"{ride.Driver.VehicleType} - {ride.Driver.VehicleId}" : "N/A",
                        CurrentStatus = GetStatusText(ride.Status),
                        EstimatedArrival = ride.Status == RideStatus.Assigned ?
                            DateTime.Now.AddMinutes(10).ToString("HH:mm") : "N/A"
                    };
                    return View(viewModel);
                }
                else
                {
                    // No driver yet - show searching state
                    var searchingViewModel = new TrackRideViewModel
                    {
                        Ride = new Ride
                        {
                            RequestId = request.RequestId,
                            PickupLocation = request.PickupLocation,
                            DropoffLocation = request.DropoffLocation,
                            Fare = request.EstimatedFare,
                            Distance = 0 // Will be calculated when driver accepts
                        },
                        DriverName = "Searching...",
                        DriverPhone = "N/A",
                        VehicleInfo = "N/A",
                        CurrentStatus = "Searching for available drivers",
                        EstimatedArrival = "N/A"
                    };
                    return View(searchingViewModel);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading ride details";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: /Customer/CancelRide/{requestId}
        [HttpPost]
        public async Task<IActionResult> CancelRide(int requestId)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var success = await _rideService.CancelRideAsync(requestId, customerId.Value);
                if (success)
                {
                    TempData["Success"] = "Ride cancelled successfully";
                }
                else
                {
                    TempData["Error"] = "Unable to cancel ride";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cancelling ride";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: /Customer/RideHistory
        public async Task<IActionResult> RideHistory()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Account");

            var rides = await _rideService.GetRideHistoryAsync(customerId.Value);
            return View(rides);
        }

        // Helper method to convert ride status to user-friendly text
        private string GetStatusText(RideStatus status)
        {
            return status switch
            {
                RideStatus.Assigned => "Driver is on the way",
                RideStatus.InProgress => "Ride in progress",
                RideStatus.Completed => "Ride completed",
                _ => "Unknown status"
            };
        }
    }
}