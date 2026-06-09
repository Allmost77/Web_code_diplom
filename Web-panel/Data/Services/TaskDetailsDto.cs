namespace Diplom.Data.Services;

public sealed class TaskDetailsDto
{
    public int Id { get; set; }
    public int LightingObjectId { get; set; }
    public string LightingObjectInventoryNumber { get; set; } = "";
    public string LightingObjectAddress { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "";
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string CreatedByUser { get; set; } = "";
    public string Executor { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
}
