using Application.Models;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Events;

/// <summary>
/// Service for managing calendar events.
/// </summary>
public class EventService : IEventService
{
    private readonly CalendarDbContext _context;

    public EventService(CalendarDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new event in the calendar.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<int> CreateEventAsync(CreateEventRequest req, CancellationToken ct)
    {
        // Validate time boundaries
        if (req.Begin >= req.End)
        {
            throw new ArgumentException("Start time must be before end time");
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

        return calendarEvent.Id;
    }

    /// <summary>
    /// Updates an existing event in the calendar.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task UpdateEventAsync(UpdateEventRequest req, CancellationToken ct)
    {
        var calendarEvent = await _context.Events.FindAsync(new object[] { req.Id }, ct);
        if (calendarEvent == null)
        {
            throw new KeyNotFoundException("Event not found");
        }

        if (req.Begin >= req.End)
        {
            throw new ArgumentException("Start time must be before end time");
        }



        calendarEvent.Name = req.Name;
        calendarEvent.Description = req.Description;
        calendarEvent.Begin = req.Begin;
        calendarEvent.End = req.End;

        await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes an event from the calendar.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task DeleteEventAsync(DeleteEventRequest req, CancellationToken ct)
    {
        var calendarEvent = await _context.Events.FindAsync(new object[] { req.Id }, ct);
        if (calendarEvent == null)
        {
            throw new KeyNotFoundException("Event not found");
        }

        _context.Events.Remove(calendarEvent);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Event>> GetEventsAsync(CancellationToken ct)
    {
        return await _context.Events.ToListAsync(ct);
    }
}

