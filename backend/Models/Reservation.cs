using System.ComponentModel.DataAnnotations;

namespace FlightReservation.Api.Models;

public class Reservation
{
    public int Id { get; set; }
    [MaxLength(80)] public string UserId { get; set; } = string.Empty;
    [MaxLength(120)] public string PassengerName { get; set; } = string.Empty;
    [MaxLength(160)] public string PassengerEmail { get; set; } = string.Empty;
    [MaxLength(8)] public string SeatNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(20)] public string Status { get; set; } = "Confirmed";
    public int FlightId { get; set; }
    public Flight? Flight { get; set; }
}
