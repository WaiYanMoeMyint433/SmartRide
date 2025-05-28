using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.ViewModels;
using SmartRide.Services;

namespace SmartRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Additional validation for driver fields
            if (model.UserType == UserType.Driver)
            {
                if (!model.VehicleType.HasValue)
                    ModelState.AddModelError("VehicleType", "Vehicle type is required for drivers");
                if (string.IsNullOrEmpty(model.VehicleId))
                    ModelState.AddModelError("VehicleId", "Vehicle ID is required for drivers");
                if (string.IsNullOrEmpty(model.LicenseNumber))
                    ModelState.AddModelError("LicenseNumber", "License number is required for drivers");

                if (!ModelState.IsValid)
                    return View(model);
            }

            try
            {
                var user = await _userService.RegisterAsync(model);

                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserType", user.UserType.ToString());
                HttpContext.Session.SetString("UserName", user.Name);

                TempData["Success"] = "Registration successful! Welcome to SmartRide.";

                // Redirect based on user type
                return user.UserType == UserType.Customer
                    ? RedirectToAction("Dashboard", "Customer")
                    : RedirectToAction("Dashboard", "Driver");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.LoginAsync(model.Email, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserType", user.UserType.ToString());
                HttpContext.Session.SetString("UserName", user.Name);

                TempData["Success"] = $"Welcome back, {user.Name}!";

                // Redirect based on user type
                return user.UserType == UserType.Customer
                    ? RedirectToAction("Dashboard", "Customer")
                    : RedirectToAction("Dashboard", "Driver");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login");
                return View(model);
            }
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully";
            return RedirectToAction("Login");
        }

        // GET: /Account/Index (redirect to login)
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
    }
}