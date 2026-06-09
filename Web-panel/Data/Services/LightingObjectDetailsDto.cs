namespace Diplom.Data.Services;

public sealed class LightingObjectDetailsDto
{
    public int Id { get; set; }
    public string InventoryNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Description { get; set; }
    public int TasksCount { get; set; }
    public int ActiveTasksCount { get; set; }
    public DateTime? LastTaskCreatedAt { get; set; }
    public List<TaskListItemDto> Tasks { get; set; } = [];
}
