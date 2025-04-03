using Application.Models;
using Application.Services.Events;
using FastEndpoints;

namespace Api.Endpoints.Events;

/// <summary>
/// Represents a request to create an event.
/// </summary>
public class CreateEventEndpoint : Endpoint<CreateEventRequest, CreateEventResponse>
{
    private readonly IEventService _service;

    public CreateEventEndpoint(IEventService service) => _service = service;

    public override void Configure()
    {
        Post("/api/events");
        AllowAnonymous();
    }

    /// <summary>
    /// Handles the creation of a new event.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public override async Task HandleAsync(CreateEventRequest req, CancellationToken ct)
    {
        try
        {
            var eventId = await _service.CreateEventAsync(req, ct);
            await SendAsync(new CreateEventResponse { Id = eventId }, cancellation: ct);
        }
        catch (ArgumentException ex)
        {
            ThrowError(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
