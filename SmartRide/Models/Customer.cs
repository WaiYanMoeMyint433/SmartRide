namespace SmartRide.Models
{
    public class Customer : User
    {
        public Customer()
        {
            UserType = UserType.Customer;
        }

        // Navigation properties
        public virtual ICollection<RideRequest> RideRequests { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
