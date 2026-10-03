using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlightReservation.Api.Models;

public class Flight
{
    public int Id { get; set; }
    [MaxLength(10)] public string FlightNumber { get; set; } = string.Empty;
    [MaxLength(100)] public string Airline { get; set; } = string.Empty;
    [MaxLength(80)] public string Origin { get; set; } = string.Empty;
    [MaxLength(80)] public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int TotalSeats { get; set; }
    [JsonIgnore]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
