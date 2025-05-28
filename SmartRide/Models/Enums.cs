namespace SmartRide.Models
{
    public enum UserType
    {
        Customer = 1,
        Driver = 2,
        Admin = 3
    }

    public enum VehicleType
    {
        Car = 1,
        Motorbike = 2
    }

    public enum DriverStatus
    {
        Offline = 1,
        Online = 2,
        Busy = 3
    }

    public enum RequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Cancelled = 3,
        Expired = 4
    }

    public enum RideStatus
    {
        Assigned = 1,
        InProgress = 2,
        Completed = 3
    }

    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        DigitalWallet = 4
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3
    }
}
