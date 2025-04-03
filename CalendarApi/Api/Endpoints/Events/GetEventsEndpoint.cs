using Application.Models;
using Application.Services.Events;
using FastEndpoints;

namespace Api.Endpoints.Events;

/// <summary>
/// Represents a request to get all events.
/// </summary>
public class GetEventsEndpoint : EndpointWithoutRequest<GetEventsResponse>
{
    private readonly IEventService _service;

    public GetEventsEndpoint(IEventService service) => _service = service;

    public override void Configure()
    {
        Get("/api/events");
        AllowAnonymous();
    }

    /// <summary>
    /// Handles the retrieval of all events.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var events = await _service.GetEventsAsync(ct);
        await SendAsync(new GetEventsResponse { Events = events }, cancellation: ct);
    }
}
