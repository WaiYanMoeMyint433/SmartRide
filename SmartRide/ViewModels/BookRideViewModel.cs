using System.ComponentModel.DataAnnotations;
using SmartRide.Models;

namespace SmartRide.ViewModels
{
    public class BookRideViewModel
    {
        [Required(ErrorMessage = "Please select pickup location")]
        [Display(Name = "Pickup Location")]
        public string PickupLocation { get; set; }

        [Required(ErrorMessage = "Please select drop-off location")]
        [Display(Name = "Drop-off Location")]
        public string DropoffLocation { get; set; }

        [Required(ErrorMessage = "Please select vehicle type")]
        [Display(Name = "Vehicle Type")]
        public VehicleType VehicleType { get; set; }

        [Required(ErrorMessage = "Please select payment method")]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Estimated Fare")]
        public decimal EstimatedFare { get; set; }

        public List<string> AvailableLocations { get; set; } = new()
        {
            "City Center",
            "Airport",
            "University",
            "Shopping Mall",
            "Hospital"
        };
    }
}
