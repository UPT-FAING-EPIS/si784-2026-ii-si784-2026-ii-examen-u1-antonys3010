using FlightReservation.Api.Data;
using FlightReservation.Api.DTOs;
using FlightReservation.Api.Models;
using FlightReservation.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Tests;

public class ReservationServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_CreatesReservation_WhenSeatIsAvailable()
    {
        await using var db = CreateDb();
        db.Flights.Add(new Flight { Id = 1, FlightNumber = "T100", Airline = "TestAir", Origin = "Tacna", Destination = "Lima", DepartureTime = DateTime.UtcNow.AddDays(1), ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(2), Price = 100, TotalSeats = 10 });
        await db.SaveChangesAsync();
        var service = new ReservationService(db);

        var (reservation, error) = await service.CreateAsync(new CreateReservationDto("demo", 1, "Antony Test", "test@example.com", "1A"));

        Assert.Null(error);
        Assert.NotNull(reservation);
        Assert.Equal("1A", reservation!.SeatNumber);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateSeat()
    {
        await using var db = CreateDb();
        db.Flights.Add(new Flight { Id = 1, FlightNumber = "T100", Airline = "TestAir", Origin = "Tacna", Destination = "Lima", DepartureTime = DateTime.UtcNow.AddDays(1), ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(2), Price = 100, TotalSeats = 10 });
        db.Reservations.Add(new Reservation { UserId = "u1", FlightId = 1, PassengerName = "A", PassengerEmail = "a@a.com", SeatNumber = "2B", Status = "Confirmed" });
        await db.SaveChangesAsync();
        var service = new ReservationService(db);

        var (reservation, error) = await service.CreateAsync(new CreateReservationDto("u2", 1, "B", "b@b.com", "2B"));

        Assert.Null(reservation);
        Assert.Equal("Seat is already reserved.", error);
    }

    [Fact]
    public async Task CancelAsync_ChangesStatus()
    {
        await using var db = CreateDb();
        db.Reservations.Add(new Reservation { Id = 5, UserId = "u", FlightId = 99, PassengerName = "A", PassengerEmail = "a@a.com", SeatNumber = "3C", Status = "Confirmed" });
        await db.SaveChangesAsync();
        var service = new ReservationService(db);

        var result = await service.CancelAsync(5);

        Assert.True(result);
        Assert.Equal("Cancelled", (await db.Reservations.FindAsync(5))!.Status);
    }
}
