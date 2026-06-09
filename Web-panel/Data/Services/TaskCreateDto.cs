namespace Diplom.Data.Services;

public sealed class TaskCreateDto
{
    public int LightingObjectId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int StatusId { get; set; }
    public string TaskType { get; set; } = "";
    public string Priority { get; set; } = "";
    public string ProblemDescription { get; set; } = "";
    public string? OperatorComment { get; set; }
    public DateTime? Deadline { get; set; }
}
