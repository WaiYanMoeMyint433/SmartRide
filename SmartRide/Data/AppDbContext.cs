// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SmartRide.Models;

namespace SmartRide.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet properties - these become database tables
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<User>()
                .HasDiscriminator<UserType>("UserType")
                .HasValue<Customer>(UserType.Customer)
                .HasValue<Driver>(UserType.Driver);

           
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Driver-specific configuration 
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.Property(e => e.VehicleId).HasMaxLength(50);
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            });

            // RideRequest entity configuration 
            modelBuilder.Entity<RideRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.PickupLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DropoffLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EstimatedFare).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Driver)
                    .WithMany()
                    .HasForeignKey(e => e.DriverId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);
            });

          
            modelBuilder.Entity<Ride>(entity =>
            {
                entity.HasKey(e => e.RideId);
                entity.Property(e => e.PickupLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DropoffLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Distance).HasColumnType("decimal(8,2)");
                entity.Property(e => e.Fare).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.RideRequest)
                    .WithOne(r => r.Ride)
                    .HasForeignKey<Ride>(e => e.RequestId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Driver)
                    .WithMany()
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

         
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");

            
                entity.HasOne(e => e.Ride)
                    .WithOne(r => r.Payment)
                    .HasForeignKey<Payment>(e => e.RideId)
                    .OnDelete(DeleteBehavior.NoAction);
  
             
            });

        
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(12,2)");
            });
        }
    }
}