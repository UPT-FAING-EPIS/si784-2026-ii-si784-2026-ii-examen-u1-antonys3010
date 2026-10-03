using FlightReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Api.Controllers;

[ApiController]
[Route("flights")]
public class FlightsController(IFlightService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? origin, [FromQuery] string? destination, [FromQuery] DateTime? date) =>
        Ok(await service.SearchAsync(origin, destination, date));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var flight = await service.GetByIdAsync(id);
        return flight is null ? NotFound() : Ok(flight);
    }
}
