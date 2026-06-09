namespace Diplom.Data.Models;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = "";
    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskReport> TaskReports { get; set; } = new List<TaskReport>();
}
