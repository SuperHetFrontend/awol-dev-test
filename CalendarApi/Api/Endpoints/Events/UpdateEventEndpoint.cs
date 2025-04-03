using Application.Models;
using Application.Services.Events;
using FastEndpoints;

namespace Api.Endpoints.Events;
/// <summary>
/// Represents a request to update an event.
/// </summary>
public class UpdateEventEndpoint : Endpoint<UpdateEventRequest>
{
    private readonly IEventService _service;

    public UpdateEventEndpoint(IEventService service) => _service = service;

    public override void Configure()
    {
        Put("/api/events/{Id}");
        AllowAnonymous();
    }

    /// <summary>
    /// Handles the update of an event.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public override async Task HandleAsync(UpdateEventRequest req, CancellationToken ct)
    {
        try
        {
            await _service.UpdateEventAsync(req, ct);
            await SendOkAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            await SendNotFoundAsync(ct);
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
