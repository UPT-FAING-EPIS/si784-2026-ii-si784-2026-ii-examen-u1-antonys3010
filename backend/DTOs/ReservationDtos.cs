using System.ComponentModel.DataAnnotations;

namespace FlightReservation.Api.DTOs;

public record CreateReservationDto(
    [Required, MaxLength(80)] string UserId,
    [Required] int FlightId,
    [Required, MaxLength(120)] string PassengerName,
    [Required, EmailAddress, MaxLength(160)] string PassengerEmail,
    [Required, RegularExpression("^[1-9][0-9]?[A-F]$", ErrorMessage = "Seat must look like 1A or 12F.")] string SeatNumber);

public record UpdateReservationDto(
    [Required, MaxLength(120)] string PassengerName,
    [Required, EmailAddress, MaxLength(160)] string PassengerEmail,
    [Required, RegularExpression("^[1-9][0-9]?[A-F]$", ErrorMessage = "Seat must look like 1A or 12F.")] string SeatNumber);

public record FlightSummaryDto(
    int Id,
    string FlightNumber,
    string Airline,
    string Origin,
    string Destination,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    decimal Price);

public record ReservationResponseDto(
    int Id,
    string UserId,
    int FlightId,
    string PassengerName,
    string PassengerEmail,
    string SeatNumber,
    string Status,
    DateTime CreatedAt,
    FlightSummaryDto? Flight);
