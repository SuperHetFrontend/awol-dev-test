namespace Application.Models;

/// <summary>
/// A DTO for updating an existing event.
/// </summary>
public class UpdateEventRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}
