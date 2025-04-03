using Application.Models;
using Domain.Models;

namespace Application.Services.Events
{
    public interface IEventService
    {
        Task<int> CreateEventAsync(CreateEventRequest req, CancellationToken ct);
        Task DeleteEventAsync(DeleteEventRequest req, CancellationToken ct);
        Task<List<Event>> GetEventsAsync(CancellationToken ct);
        Task UpdateEventAsync(UpdateEventRequest req, CancellationToken ct);
    }
}