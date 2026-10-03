using FlightReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>().Property(f => f.Price).HasPrecision(10, 2);
        modelBuilder.Entity<Flight>().HasIndex(f => new { f.Origin, f.Destination, f.DepartureTime });
        modelBuilder.Entity<Reservation>().HasIndex(r => new { r.FlightId, r.SeatNumber }).IsUnique().HasFilter("\"Status\" = 'Confirmed'");
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Flight)
            .WithMany(f => f.Reservations)
            .HasForeignKey(r => r.FlightId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
