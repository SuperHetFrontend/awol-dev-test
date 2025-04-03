namespace Application.Models;

/// <summary>
/// A DTO for creating a new event.
/// </summary>
public class CreateEventRequest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}
