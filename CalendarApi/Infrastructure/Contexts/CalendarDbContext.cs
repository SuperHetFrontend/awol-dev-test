using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

/// <summary>
/// Represents a database context for calendar events.
/// </summary>
public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>().HasData(
            new Event { Id = 1, Name = "Event A", Description = "Evt A description", Begin = DateTime.Now, End = DateTime.Now.AddHours(1) },
            new Event { Id = 2, Name = "Event B", Description = "Evt B description", Begin = DateTime.Now.AddHours(1), End = DateTime.Now.AddHours(2) },
            new Event { Id = 3, Name = "Event C", Description = "Evt B description", Begin = DateTime.Now.AddHours(2), End = DateTime.Now.AddHours(3) }
        );
    }

    public virtual DbSet<Event> Events => Set<Event>();
}
