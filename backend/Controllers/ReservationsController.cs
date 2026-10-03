using FlightReservation.Api.DTOs;
using FlightReservation.Api.Models;
using FlightReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Api.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController(IReservationService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var (reservation, error) = await service.CreateAsync(dto);
        if (reservation is null) return BadRequest(new { message = error });
        return Created($"/reservations/{reservation.Id}", ToDto(reservation));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var reservations = await service.GetByUserAsync(userId);
        return Ok(reservations.Select(ToDto));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        var (reservation, error) = await service.UpdateAsync(id, dto);
        return reservation is null ? BadRequest(new { message = error }) : Ok(ToDto(reservation));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id) => await service.CancelAsync(id) ? NoContent() : NotFound();

    private static ReservationResponseDto ToDto(Reservation r) => new(
        r.Id,
        r.UserId,
        r.FlightId,
        r.PassengerName,
        r.PassengerEmail,
        r.SeatNumber,
        r.Status,
        r.CreatedAt,
        r.Flight is null ? null : new FlightSummaryDto(
            r.Flight.Id,
            r.Flight.FlightNumber,
            r.Flight.Airline,
            r.Flight.Origin,
            r.Flight.Destination,
            r.Flight.DepartureTime,
            r.Flight.ArrivalTime,
            r.Flight.Price));
}
