using FlightReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var today = DateTime.UtcNow.Date;

        var flights = new List<Flight>
        {
            F("LA2101", "LATAM", "Tacna", "Lima", today.AddDays(1).AddHours(6),  today.AddDays(1).AddHours(7).AddMinutes(50), 189.90m),
            F("JA7042", "JetSMART", "Tacna", "Lima", today.AddDays(1).AddHours(9),  today.AddDays(1).AddHours(10).AddMinutes(45), 149.90m),
            F("H22101", "Sky Airline", "Tacna", "Lima", today.AddDays(1).AddHours(13), today.AddDays(1).AddHours(14).AddMinutes(50), 159.90m),
            F("LA2115", "LATAM", "Tacna", "Lima", today.AddDays(1).AddHours(18), today.AddDays(1).AddHours(19).AddMinutes(50), 209.90m),
            F("JA7050", "JetSMART", "Tacna", "Lima", today.AddDays(2).AddHours(7),  today.AddDays(2).AddHours(8).AddMinutes(45), 139.90m),
            F("H22115", "Sky Airline", "Tacna", "Lima", today.AddDays(2).AddHours(11), today.AddDays(2).AddHours(12).AddMinutes(50), 154.90m),
            F("LA2121", "LATAM", "Tacna", "Lima", today.AddDays(2).AddHours(16), today.AddDays(2).AddHours(17).AddMinutes(50), 199.90m),
            F("JA7064", "JetSMART", "Tacna", "Lima", today.AddDays(3).AddHours(8),  today.AddDays(3).AddHours(9).AddMinutes(45), 144.90m),

            F("LA2401", "LATAM", "Lima", "Tacna", today.AddDays(1).AddHours(7),  today.AddDays(1).AddHours(8).AddMinutes(50), 184.90m),
            F("H22405", "Sky Airline", "Lima", "Tacna", today.AddDays(1).AddHours(14), today.AddDays(1).AddHours(15).AddMinutes(50), 169.90m),
            F("JA7402", "JetSMART", "Lima", "Tacna", today.AddDays(2).AddHours(17), today.AddDays(2).AddHours(18).AddMinutes(45), 149.90m),

            F("H22110", "Sky Airline", "Lima", "Cusco", today.AddDays(2).AddHours(8),  today.AddDays(2).AddHours(9).AddMinutes(25), 169.00m),
            F("LA2012", "LATAM", "Lima", "Cusco", today.AddDays(2).AddHours(12), today.AddDays(2).AddHours(13).AddMinutes(25), 199.00m),
            F("JA7031", "JetSMART", "Lima", "Cusco", today.AddDays(3).AddHours(15), today.AddDays(3).AddHours(16).AddMinutes(30), 159.00m),

            F("LA2020", "LATAM", "Lima", "Arequipa", today.AddDays(3).AddHours(8),  today.AddDays(3).AddHours(9).AddMinutes(35), 175.50m),
            F("H22220", "Sky Airline", "Lima", "Arequipa", today.AddDays(3).AddHours(13), today.AddDays(3).AddHours(14).AddMinutes(35), 165.50m),
            F("JA7020", "JetSMART", "Lima", "Arequipa", today.AddDays(4).AddHours(18), today.AddDays(4).AddHours(19).AddMinutes(30), 145.50m),

            F("JA7001", "JetSMART", "Arequipa", "Tacna", today.AddDays(4).AddHours(9),  today.AddDays(4).AddHours(10), 129.90m),
            F("LA2605", "LATAM", "Arequipa", "Tacna", today.AddDays(4).AddHours(16), today.AddDays(4).AddHours(17), 149.90m),
            F("H22607", "Sky Airline", "Cusco", "Lima", today.AddDays(5).AddHours(10), today.AddDays(5).AddHours(11).AddMinutes(25), 179.90m),
            F("LA2701", "LATAM", "Cusco", "Lima", today.AddDays(5).AddHours(17), today.AddDays(5).AddHours(18).AddMinutes(25), 194.90m),
            F("JA7801", "JetSMART", "Arequipa", "Lima", today.AddDays(5).AddHours(12), today.AddDays(5).AddHours(13).AddMinutes(30), 139.90m),
            F("LA2901", "LATAM", "Lima", "Piura", today.AddDays(6).AddHours(9), today.AddDays(6).AddHours(10).AddMinutes(45), 179.90m),
            F("JA7902", "JetSMART", "Lima", "Trujillo", today.AddDays(6).AddHours(14), today.AddDays(6).AddHours(15).AddMinutes(15), 129.90m)
        };

        var existingNumbers = await db.Flights.AsNoTracking().Select(f => f.FlightNumber).ToListAsync();
        var missing = flights.Where(f => !existingNumbers.Contains(f.FlightNumber)).ToList();
        if (missing.Count == 0) return;

        db.Flights.AddRange(missing);
        await db.SaveChangesAsync();
    }

    private static Flight F(string number, string airline, string origin, string destination, DateTime departure, DateTime arrival, decimal price) =>
        new()
        {
            FlightNumber = number,
            Airline = airline,
            Origin = origin,
            Destination = destination,
            DepartureTime = departure,
            ArrivalTime = arrival,
            Price = price,
            TotalSeats = 60
        };
}
