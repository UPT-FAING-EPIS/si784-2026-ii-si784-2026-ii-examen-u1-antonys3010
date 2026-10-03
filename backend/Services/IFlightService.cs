using FlightReservation.Api.Models;

namespace FlightReservation.Api.Services;

public interface IFlightService
{
    Task<IReadOnlyList<Flight>> SearchAsync(string? origin, string? destination, DateTime? date);
    Task<Flight?> GetByIdAsync(int id);
}
