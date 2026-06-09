namespace Diplom.Data.Services;

public sealed class TaskListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public string Executor { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
}
