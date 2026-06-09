namespace Diplom.Data.Models;

public class TaskReport
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int CreatedByUserId { get; set; }
    public string? Comment { get; set; }
    public string CompletionStatus { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public TaskItem Task { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<TaskReportPhoto> Photos { get; set; } = new List<TaskReportPhoto>();
}
