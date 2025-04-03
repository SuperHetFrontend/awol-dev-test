using Application.Models;
using Application.Services.Events;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Tests;

/// <summary>
/// Unit tests for the EventService.
/// </summary>
public class EventServiceTests : IDisposable
{
    private readonly CalendarDbContext _context;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        // Use a unique in-memory database for each test
        var options = new DbContextOptionsBuilder<CalendarDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CalendarDbContext(options);
        _eventService = new EventService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Tests the CreateEventAsync method of the EventService with VALID times.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateEventAsync_ShouldCreateEvent_WhenValidRequest()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            Name = "Test Event",
            Description = "Test Description",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var eventId = await _eventService.CreateEventAsync(request, cancellationToken);

        // Assert
        var createdEvent = await _context.Events.FindAsync(eventId);
        Assert.NotNull(createdEvent);
        Assert.Equal(request.Name, createdEvent.Name);
        Assert.Equal(request.Description, createdEvent.Description);
        Assert.Equal(request.Begin, createdEvent.Begin);
        Assert.Equal(request.End, createdEvent.End);
    }

    /// <summary>
    /// Tests the CreateEventAsync method of the EventService with INVALID times.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateEventAsync_ShouldThrowArgumentException_WhenBeginIsNotBeforeEnd()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            Name = "Test Event",
            Description = "Test Description",
            Begin = DateTime.Now.AddHours(2),
            End = DateTime.Now.AddHours(1)  // Invalid time boundaries
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _eventService.CreateEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the CreateEventAsync method of the EventService with overlapping events.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateEventAsync_ShouldThrowInvalidOperationException_WhenOverlappingEventExists()
    {
        // Arrange: Create an existing event that overlaps with the new request.
        var existingEvent = new Event
        {
            Name = "Existing Event",
            Description = "Existing Description",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        _context.Events.Add(existingEvent);
        await _context.SaveChangesAsync();

        var request = new CreateEventRequest
        {
            Name = "New Event",
            Description = "New Description",
            // New event overlaps with the existing event.
            Begin = DateTime.Now.AddHours(1).AddMinutes(30),
            End = DateTime.Now.AddHours(2).AddMinutes(30)
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _eventService.CreateEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the UpdateEventAsync method of the EventService with VALID times.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateEventAsync_ShouldUpdateEvent_WhenValidRequest()
    {
        // Arrange: Create an event to update.
        var existingEvent = new Event
        {
            Name = "Existing Event",
            Description = "Existing Description",
            Begin = DateTime.Now.AddHours(3),
            End = DateTime.Now.AddHours(4)
        };
        _context.Events.Add(existingEvent);
        await _context.SaveChangesAsync();

        var request = new UpdateEventRequest
        {
            Id = existingEvent.Id,
            Name = "Updated Event",
            Description = "Updated Description",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        var cancellationToken = CancellationToken.None;

        // Act
        await _eventService.UpdateEventAsync(request, cancellationToken);

        // Assert
        var updatedEvent = await _context.Events.FindAsync(existingEvent.Id);
        Assert.NotNull(updatedEvent);
        Assert.Equal(request.Name, updatedEvent.Name);
        Assert.Equal(request.Description, updatedEvent.Description);
        Assert.Equal(request.Begin, updatedEvent.Begin);
        Assert.Equal(request.End, updatedEvent.End);
    }

    /// <summary>
    /// Tests the UpdateEventAsync method of the EventService when no event is found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateEventAsync_ShouldThrowKeyNotFoundException_WhenEventNotFound()
    {
        // Arrange: Use a non-existing event id.
        var request = new UpdateEventRequest
        {
            Id = 999,
            Name = "Updated Event",
            Description = "Updated Description",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _eventService.UpdateEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the UpdateEventAsync method of the EventService for begin before end.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateEventAsync_ShouldThrowArgumentException_WhenBeginIsNotBeforeEnd()
    {
        // Arrange: Create an event.
        var existingEvent = new Event
        {
            Name = "Existing Event",
            Description = "Existing Description",
            Begin = DateTime.Now.AddHours(3),
            End = DateTime.Now.AddHours(4)
        };
        _context.Events.Add(existingEvent);
        await _context.SaveChangesAsync();

        var request = new UpdateEventRequest
        {
            Id = existingEvent.Id,
            Name = "Updated Event",
            Description = "Updated Description",
            Begin = DateTime.Now.AddHours(2),
            End = DateTime.Now.AddHours(1)  // Invalid: Begin is not before End
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _eventService.UpdateEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the UpdateEventAsync method of the EventService with overlapping events.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateEventAsync_ShouldThrowInvalidOperationException_WhenOverlappingEventExists()
    {
        // Arrange: Create two events.
        var event1 = new Event
        {
            Name = "Event 1",
            Description = "Description 1",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        var event2 = new Event
        {
            Name = "Event 2",
            Description = "Description 2",
            Begin = DateTime.Now.AddHours(3),
            End = DateTime.Now.AddHours(4)
        };
        _context.Events.AddRange(event1, event2);
        await _context.SaveChangesAsync();

        // Try to update event2 so that it overlaps with event1.
        var request = new UpdateEventRequest
        {
            Id = event2.Id,
            Name = "Updated Event 2",
            Description = "Updated Description 2",
            Begin = DateTime.Now.AddHours(1).AddMinutes(30),
            End = DateTime.Now.AddHours(2).AddMinutes(30)
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _eventService.UpdateEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the DeleteEventAsync method of the EventService.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteEventAsync_ShouldDeleteEvent_WhenEventExists()
    {
        // Arrange: Create an event to delete.
        var existingEvent = new Event
        {
            Name = "Event to Delete",
            Description = "Description",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        _context.Events.Add(existingEvent);
        await _context.SaveChangesAsync();

        var request = new DeleteEventRequest
        {
            Id = existingEvent.Id
        };
        var cancellationToken = CancellationToken.None;

        // Act
        await _eventService.DeleteEventAsync(request, cancellationToken);

        // Assert
        var deletedEvent = await _context.Events.FindAsync(existingEvent.Id);
        Assert.Null(deletedEvent);
    }

    /// <summary>
    /// Tests the DeleteEventAsync method of the EventService when no event is found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteEventAsync_ShouldThrowKeyNotFoundException_WhenEventNotFound()
    {
        // Arrange: Use a non-existing event id.
        var request = new DeleteEventRequest
        {
            Id = 999
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _eventService.DeleteEventAsync(request, cancellationToken));
    }

    /// <summary>
    /// Tests the GetEventsAsync method of the EventService.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetEventsAsync_ShouldReturnAllEvents()
    {
        // Arrange: Add some events.
        var event1 = new Event
        {
            Name = "Event 1",
            Description = "Description 1",
            Begin = DateTime.Now.AddHours(1),
            End = DateTime.Now.AddHours(2)
        };
        var event2 = new Event
        {
            Name = "Event 2",
            Description = "Description 2",
            Begin = DateTime.Now.AddHours(3),
            End = DateTime.Now.AddHours(4)
        };
        _context.Events.AddRange(event1, event2);
        await _context.SaveChangesAsync();

        var cancellationToken = CancellationToken.None;

        // Act
        var events = await _eventService.GetEventsAsync(cancellationToken);

        // Assert
        Assert.NotNull(events);
        Assert.Equal(2, events.Count);
    }
}