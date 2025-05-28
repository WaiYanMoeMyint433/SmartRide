using System.ComponentModel.DataAnnotations;
using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select user type")]
        [Display(Name = "I am a")]
        public UserType UserType { get; set; }

        // Driver-specific fields
        [Display(Name = "Vehicle Type")]
        public VehicleType? VehicleType { get; set; }

        [Display(Name = "Vehicle ID")]
        public string? VehicleId { get; set; }

        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }
    }
}
