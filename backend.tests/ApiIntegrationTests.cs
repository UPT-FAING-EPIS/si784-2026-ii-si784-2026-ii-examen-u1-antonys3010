using System.Net.Http.Json;
using FlightReservation.Api.Data;
using FlightReservation.Api.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlightReservation.Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase($"integration-flights-{Guid.NewGuid()}"));
        });
    }
}

public class ApiIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public ApiIntegrationTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetFlights_ReturnsSuccessAndSeedData()
    {
        var response = await _client.GetAsync("/flights");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tacna", body);
    }

    [Fact]
    public async Task CreateReservation_ThenGetByUser_ReturnsCreatedReservation()
    {
        var flights = await _client.GetFromJsonAsync<List<FlightResult>>("/flights?origin=Tacna&destination=Lima");
        Assert.NotNull(flights);
        Assert.NotEmpty(flights!);

        var request = new CreateReservationDto(
            "integration-user",
            flights![0].Id,
            "Integration Test",
            "integration@test.com",
            "9F");

        var create = await _client.PostAsJsonAsync("/reservations", request);
        create.EnsureSuccessStatusCode();

        var reservations = await _client.GetFromJsonAsync<List<ReservationResponseDto>>("/reservations/integration-user");
        Assert.NotNull(reservations);
        Assert.Contains(reservations!, r => r.UserId == "integration-user" && r.SeatNumber == "9F");
    }

    private sealed record FlightResult(int Id);
}
