using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

/// <summary>
/// Represents a calendar event.
/// </summary>
public class Event
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "";
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
}
