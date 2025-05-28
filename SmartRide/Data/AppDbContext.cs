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

            // Configure User inheritance (Table Per Hierarchy) - KEEP THIS
            modelBuilder.Entity<User>()
                .HasDiscriminator<UserType>("UserType")
                .HasValue<Customer>(UserType.Customer)
                .HasValue<Driver>(UserType.Driver);

            // User entity configuration - KEEP THIS
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Driver-specific configuration - KEEP THIS
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.Property(e => e.VehicleId).HasMaxLength(50);
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            });

            // RideRequest entity configuration - FIXED
            modelBuilder.Entity<RideRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.PickupLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DropoffLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EstimatedFare).HasColumnType("decimal(10,2)");

                // ✅ EXPLICIT configuration to control cascade behavior
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

            // Ride entity configuration - FIXED
            modelBuilder.Entity<Ride>(entity =>
            {
                entity.HasKey(e => e.RideId);
                entity.Property(e => e.PickupLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DropoffLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Distance).HasColumnType("decimal(8,2)");
                entity.Property(e => e.Fare).HasColumnType("decimal(10,2)");

                // ✅ Keep the RideRequest relationship
                entity.HasOne(e => e.RideRequest)
                    .WithOne(r => r.Ride)
                    .HasForeignKey<Ride>(e => e.RequestId)
                    .OnDelete(DeleteBehavior.NoAction);

                // ✅ EXPLICIT configuration to prevent cascade conflicts
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Driver)
                    .WithMany()
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Payment entity configuration - SIMPLIFIED
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");

                // ✅ Keep only the Ride relationship with NO ACTION
                entity.HasOne(e => e.Ride)
                    .WithOne(r => r.Payment)
                    .HasForeignKey<Payment>(e => e.RideId)
                    .OnDelete(DeleteBehavior.NoAction);

                // ✅ If you have Customer relationship in Payment, configure it too
                // entity.HasOne<Customer>()
                //     .WithMany()
                //     .HasForeignKey(e => e.CustomerId)
                //     .OnDelete(DeleteBehavior.NoAction);
            });

            // Report entity configuration - KEEP THIS
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(12,2)");
            });
        }
    }
}