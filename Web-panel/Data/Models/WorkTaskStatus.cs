namespace Diplom.Data.Models;

public class WorkTaskStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
