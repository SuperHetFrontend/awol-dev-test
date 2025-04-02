using Domain.Models;
using FastEndpoints;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public class GetEventsResponse
{
    public List<Event> Events { get; set; } = new();
}

/// <summary>
/// Represents a request to get all events.
/// </summary>
public class GetEventsEndpoint : EndpointWithoutRequest<GetEventsResponse>
{
    private readonly CalendarDbContext _context;

    public GetEventsEndpoint(CalendarDbContext context) => _context = context;

    public override void Configure()
    {
        Get("/api/events");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var events = await _context.Events.ToListAsync(ct);
        await SendAsync(new GetEventsResponse { Events = events }, cancellation: ct);
    }
}
