using Microsoft.EntityFrameworkCore;
using busticketbooking.Models;
using busticketbooking.Models;

namespace busticketbooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<busticketbooking.Models.BusRoute> Routes { get; set; } // Fully qualified name
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed Admin Data
       modelBuilder.Entity<Admin>().HasData(
           new Admin
           {
               AdminID = 1,
               Name = "Super Admin",
               Email = "Admin@gmail.com",
               Password = "Admin@123" // Note: Password should ideally be hashed
           }
       );

            // Passenger - Booking relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PassengerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Schedule - Booking relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleID)
                .OnDelete(DeleteBehavior.Cascade);

            // Bus - Schedule relationship
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Schedules)
                .HasForeignKey(s => s.BusID)
                .OnDelete(DeleteBehavior.Cascade);

            // Route - Schedule relationship
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteID)
                .OnDelete(DeleteBehavior.Cascade);

            // Bus - Seat relationship
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BusID)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking - Payment relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure unique email for Admin
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // Ensure unique email for Passenger
            modelBuilder.Entity<Passenger>()
                .HasIndex(p => p.Email)
                .IsUnique();
        }

    }
}
