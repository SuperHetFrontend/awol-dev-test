using Domain.Models;

namespace Application.Models;

/// <summary>
/// A DTO representing the response for getting all events.
/// </summary>
public class GetEventsResponse
{
    public List<Event> Events { get; set; } = new();
}
