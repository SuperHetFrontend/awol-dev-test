using Domain.Models;
using FastEndpoints;
using Infrastructure.Contexts;

namespace Api.Endpoints;

public class CreateEventRequest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}

public class CreateEventResponse
{
    public int Id { get; set; }
}

/// <summary>
/// Represents a request to create an event.
/// </summary>
public class CreateEventEndpoint : Endpoint<CreateEventRequest, CreateEventResponse>
{
    private readonly CalendarDbContext _context;

    public CreateEventEndpoint(CalendarDbContext context) => _context = context;

    public override void Configure()
    {
        Post("/api/events");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateEventRequest req, CancellationToken ct)
    {
        if (req.Begin >= req.End)
        {
            ThrowError("Start time must be before end time");
            return;
        }

        var calendarEvent = new Event
        {
            Name = req.Name,
            Description = req.Description,
            Begin = req.Begin,
            End = req.End
        };

        _context.Events.Add(calendarEvent);

        await _context.SaveChangesAsync(ct);
        await SendAsync(new CreateEventResponse { Id = calendarEvent.Id }, cancellation: ct);
    }
}
