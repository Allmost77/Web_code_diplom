namespace Diplom.Data.Models;

public class TaskReportPhoto
{
    public int Id { get; set; }
    public int TaskReportId { get; set; }
    public string FilePath { get; set; } = "";

    public TaskReport TaskReport { get; set; } = null!;
}
