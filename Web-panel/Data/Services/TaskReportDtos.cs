using Microsoft.AspNetCore.Http;

namespace Diplom.Data.Services;

public sealed class TaskReportCreateDto
{
    public int TaskId { get; set; }
    public int CreatedByUserId { get; set; }
    public string? Comment { get; set; }
    public string CompletionStatus { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public IReadOnlyList<IFormFile> Photos { get; set; } = [];
}

public sealed class TaskReportDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string? Comment { get; set; }
    public string CompletionStatus { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskReportPhotoDto> Photos { get; set; } = [];
}

public sealed class TaskReportListItemDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string? Comment { get; set; }
    public string CompletionStatus { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PhotoCount { get; set; }
}

public sealed class TaskReportDetailsDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = "";
    public string LightingObjectInventoryNumber { get; set; } = "";
    public string LightingObjectAddress { get; set; } = "";
    public string? Comment { get; set; }
    public string CompletionStatus { get; set; } = "";
    public DateTime FormedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskReportPhotoDto> Photos { get; set; } = [];
}

public sealed class TaskReportPhotoDto
{
    public int Id { get; set; }
    public string FilePath { get; set; } = "";
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
}

public sealed record TaskReportCreateResult(
    TaskReportCreateStatus Status,
    TaskReportDto? Report = null,
    string? ErrorMessage = null);

public enum TaskReportCreateStatus
{
    Success,
    TaskNotFound,
    Forbidden,
    InvalidPhoto
}
