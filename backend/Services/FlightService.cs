using FlightReservation.Api.Data;
using FlightReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Services;

public class FlightService(AppDbContext db) : IFlightService
{
    public async Task<IReadOnlyList<Flight>> SearchAsync(string? origin, string? destination, DateTime? date)
    {
        var query = db.Flights.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(origin)) query = query.Where(f => f.Origin.ToLower() == origin.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(destination)) query = query.Where(f => f.Destination.ToLower() == destination.Trim().ToLower());
        if (date.HasValue)
        {
            var start = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(f => f.DepartureTime >= start && f.DepartureTime < end);
        }
        return await query.OrderBy(f => f.DepartureTime).ToListAsync();
    }

    public Task<Flight?> GetByIdAsync(int id) => db.Flights.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
}
