using FastEndpoints;
using Infrastructure.Contexts;

namespace Api.Endpoints;

public class DeleteEventRequest
{
    public int Id { get; set; }
}

/// <summary>
/// Represents a request to delete an event.
/// </summary>
public class DeleteEventEndpoint : Endpoint<DeleteEventRequest>
{
    private readonly CalendarDbContext _context;

    public DeleteEventEndpoint(CalendarDbContext context) => _context = context;

    public override void Configure()
    {
        Delete("/api/events/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteEventRequest req, CancellationToken ct)
    {
        var calendarEvent = await _context.Events.FindAsync(new object[] { req.Id }, ct);
        if (calendarEvent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _context.Events.Remove(calendarEvent);

        await _context.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
