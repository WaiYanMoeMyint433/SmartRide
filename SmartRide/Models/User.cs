using System.ComponentModel.DataAnnotations;

namespace SmartRide.Models
{
    public abstract class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        public UserType UserType { get; set; }
    }
}
