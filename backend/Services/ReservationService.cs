using FlightReservation.Api.Data;
using FlightReservation.Api.DTOs;
using FlightReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Services;

public class ReservationService(AppDbContext db) : IReservationService
{
    public async Task<(Reservation? reservation, string? error)> CreateAsync(CreateReservationDto dto)
    {
        var flight = await db.Flights.FindAsync(dto.FlightId);
        if (flight is null) return (null, "Flight not found.");
        if (flight.DepartureTime <= DateTime.UtcNow) return (null, "This flight has already departed.");

        var taken = await db.Reservations.AnyAsync(r => r.FlightId == dto.FlightId && r.SeatNumber == dto.SeatNumber.ToUpper() && r.Status == "Confirmed");
        if (taken) return (null, "Seat is already reserved.");

        var confirmedCount = await db.Reservations.CountAsync(r => r.FlightId == dto.FlightId && r.Status == "Confirmed");
        if (confirmedCount >= flight.TotalSeats) return (null, "Flight is full.");

        var reservation = new Reservation
        {
            UserId = dto.UserId.Trim(), FlightId = dto.FlightId, PassengerName = dto.PassengerName.Trim(),
            PassengerEmail = dto.PassengerEmail.Trim().ToLowerInvariant(), SeatNumber = dto.SeatNumber.Trim().ToUpperInvariant()
        };
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();
        await db.Entry(reservation).Reference(r => r.Flight).LoadAsync();
        return (reservation, null);
    }

    public async Task<IReadOnlyList<Reservation>> GetByUserAsync(string userId) =>
        await db.Reservations.AsNoTracking().Include(r => r.Flight).Where(r => r.UserId.ToLower() == userId.Trim().ToLower())
            .OrderByDescending(r => r.CreatedAt).ToListAsync();

    public async Task<(Reservation? reservation, string? error)> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await db.Reservations.Include(r => r.Flight).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null || reservation.Status != "Confirmed") return (null, "Active reservation not found.");

        var normalizedSeat = dto.SeatNumber.Trim().ToUpperInvariant();
        var taken = await db.Reservations.AnyAsync(r => r.Id != id && r.FlightId == reservation.FlightId && r.SeatNumber == normalizedSeat && r.Status == "Confirmed");
        if (taken) return (null, "Seat is already reserved.");

        reservation.PassengerName = dto.PassengerName.Trim();
        reservation.PassengerEmail = dto.PassengerEmail.Trim().ToLowerInvariant();
        reservation.SeatNumber = normalizedSeat;
        await db.SaveChangesAsync();
        return (reservation, null);
    }

    public async Task<bool> CancelAsync(int id)
    {
        var reservation = await db.Reservations.FindAsync(id);
        if (reservation is null || reservation.Status == "Cancelled") return false;
        reservation.Status = "Cancelled";
        await db.SaveChangesAsync();
        return true;
    }
}
