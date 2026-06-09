namespace Diplom.Data.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int LightingObjectId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int StatusId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }

    public LightingObject LightingObject { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public User? AssignedToUser { get; set; }
    public WorkTaskStatus Status { get; set; } = null!;
    public ICollection<TaskReport> Reports { get; set; } = new List<TaskReport>();
}
