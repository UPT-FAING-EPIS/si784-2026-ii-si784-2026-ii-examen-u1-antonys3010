using FlightReservation.Api.DTOs;
using FlightReservation.Api.Models;

namespace FlightReservation.Api.Services;

public interface IReservationService
{
    Task<(Reservation? reservation, string? error)> CreateAsync(CreateReservationDto dto);
    Task<IReadOnlyList<Reservation>> GetByUserAsync(string userId);
    Task<(Reservation? reservation, string? error)> UpdateAsync(int id, UpdateReservationDto dto);
    Task<bool> CancelAsync(int id);
}
