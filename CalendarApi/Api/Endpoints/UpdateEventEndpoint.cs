using FastEndpoints;
using Infrastructure.Contexts;

namespace Api.Endpoints;
/// <summary>
/// Represents a request to update an event.
/// </summary>
public class UpdateEventRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}

public class UpdateEventEndpoint : Endpoint<UpdateEventRequest>
{
    private readonly CalendarDbContext _context;

    public UpdateEventEndpoint(CalendarDbContext context) => _context = context;

    public override void Configure()
    {
        Put("/api/events/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateEventRequest req, CancellationToken ct)
    {
        var calendarEvent = await _context.Events.FindAsync(new object[] { req.Id }, ct);
        if (calendarEvent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (req.Begin >= req.End)
        {
            ThrowError("Start time must be before end time");
            return;
        }

        calendarEvent.Name = req.Name;
        calendarEvent.Description = req.Description;
        calendarEvent.Begin = req.Begin;
        calendarEvent.End = req.End;

        await _context.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
