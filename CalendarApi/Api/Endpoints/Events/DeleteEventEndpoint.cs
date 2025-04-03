using Application.Models;
using Application.Services.Events;
using FastEndpoints;

namespace Api.Endpoints.Events;

/// <summary>
/// Represents a request to delete an event.
/// </summary>
public class DeleteEventEndpoint : Endpoint<DeleteEventRequest>
{
    private readonly IEventService _service;

    public DeleteEventEndpoint(IEventService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/events/{Id}");
        AllowAnonymous();
    }

    /// <summary>
    /// Handles the deletion of an event.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public override async Task HandleAsync(DeleteEventRequest req, CancellationToken ct)
    {
        try
        {
            await _service.DeleteEventAsync(req, ct);
            await SendOkAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            await SendNotFoundAsync(ct);
        }
    }
}
