namespace Diplom.Data.Services;

public sealed class TaskEditDto
{
    public int Id { get; set; }
    public int LightingObjectId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? Deadline { get; set; }
}
