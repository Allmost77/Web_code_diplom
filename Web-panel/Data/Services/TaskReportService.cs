using Diplom.Data.DbContext;
using Diplom.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Diplom.Data.Services;

public sealed class TaskReportService(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IWebHostEnvironment environment)
{
    private const long MaxPhotoBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedPhotoExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".heic",
        ".heif"
    ];

    public async Task<TaskReportCreateResult> CreateReportAsync(TaskReportCreateDto createDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var task = await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == createDto.TaskId)
            .Select(task => new
            {
                task.Id,
                task.AssignedToUserId
            })
            .FirstOrDefaultAsync();

        if (task is null)
            return new TaskReportCreateResult(TaskReportCreateStatus.TaskNotFound);

        if (task.AssignedToUserId != createDto.CreatedByUserId)
            return new TaskReportCreateResult(TaskReportCreateStatus.Forbidden);

        foreach (var photo in createDto.Photos)
        {
            var validationError = ValidatePhoto(photo);

            if (validationError is not null)
                return new TaskReportCreateResult(TaskReportCreateStatus.InvalidPhoto, ErrorMessage: validationError);
        }

        var savedFiles = new List<string>();

        try
        {
            var now = ToDatabaseTimestamp(DateTime.Now);
            var report = new TaskReport
            {
                TaskId = createDto.TaskId,
                CreatedByUserId = createDto.CreatedByUserId,
                Comment = TrimToNull(createDto.Comment),
                CompletionStatus = createDto.CompletionStatus.Trim(),
                FormedAt = ToDatabaseTimestamp(createDto.FormedAt),
                CreatedAt = now
            };

            foreach (var photo in createDto.Photos.Where(photo => photo.Length > 0))
            {
                var photoRecord = await SavePhotoAsync(photo, savedFiles);
                report.Photos.Add(photoRecord);
            }

            dbContext.TaskReports.Add(report);
            await dbContext.SaveChangesAsync();

            return new TaskReportCreateResult(TaskReportCreateStatus.Success, MapReport(report));
        }
        catch
        {
            DeleteSavedFiles(savedFiles);
            throw;
        }
    }

    public async Task<List<TaskReportListItemDto>> GetReportsForTaskAsync(int taskId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.TaskReports
            .AsNoTracking()
            .Where(report => report.TaskId == taskId)
            .OrderByDescending(report => report.FormedAt)
            .ThenByDescending(report => report.Id)
            .Select(report => new TaskReportListItemDto
            {
                Id = report.Id,
                TaskId = report.TaskId,
                Comment = report.Comment,
                CompletionStatus = report.CompletionStatus,
                FormedAt = report.FormedAt,
                CreatedAt = report.CreatedAt,
                PhotoCount = report.Photos.Count
            })
            .ToListAsync();
    }

    public async Task<TaskReportDetailsDto?> GetReportDetailsAsync(int reportId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.TaskReports
            .AsNoTracking()
            .Where(report => report.Id == reportId)
            .Select(report => new TaskReportDetailsDto
            {
                Id = report.Id,
                TaskId = report.TaskId,
                TaskTitle = report.Task.Title,
                LightingObjectInventoryNumber = report.Task.LightingObject.InventoryNumber,
                LightingObjectAddress = report.Task.LightingObject.Address,
                Comment = report.Comment,
                CompletionStatus = report.CompletionStatus,
                FormedAt = report.FormedAt,
                CreatedAt = report.CreatedAt,
                Photos = report.Photos
                    .OrderBy(photo => photo.Id)
                    .Select(photo => new TaskReportPhotoDto
                    {
                        Id = photo.Id,
                        FilePath = photo.FilePath
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    private async Task<TaskReportPhoto> SavePhotoAsync(
        IFormFile photo,
        List<string> savedFiles)
    {
        var photosDirectory = Path.Combine(environment.ContentRootPath, "photos");
        Directory.CreateDirectory(photosDirectory);

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(photosDirectory, fileName);

        await using (var stream = File.Create(absolutePath))
        {
            await photo.CopyToAsync(stream);
        }

        savedFiles.Add(absolutePath);

        return new TaskReportPhoto
        {
            FilePath = $"/photos/{fileName}"
        };
    }

    private static string? ValidatePhoto(IFormFile photo)
    {
        if (photo.Length == 0)
            return null;

        if (photo.Length > MaxPhotoBytes)
            return $"Photo '{photo.FileName}' is larger than 10 MB.";

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) || !AllowedPhotoExtensions.Contains(extension))
            return $"Photo '{photo.FileName}' has unsupported extension.";

        if (!string.IsNullOrWhiteSpace(photo.ContentType) &&
            !photo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return $"File '{photo.FileName}' is not an image.";
        }

        return null;
    }

    private static TaskReportDto MapReport(TaskReport report) =>
        new()
        {
            Id = report.Id,
            TaskId = report.TaskId,
            Comment = report.Comment,
            CompletionStatus = report.CompletionStatus,
            FormedAt = report.FormedAt,
            CreatedAt = report.CreatedAt,
            Photos = report.Photos
                .OrderBy(photo => photo.Id)
                .Select(photo => new TaskReportPhotoDto
                {
                    Id = photo.Id,
                    FilePath = photo.FilePath
                })
                .ToList()
        };

    private static DateTime ToDatabaseTimestamp(DateTime value) =>
        DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void DeleteSavedFiles(IEnumerable<string> savedFiles)
    {
        foreach (var savedFile in savedFiles)
        {
            try
            {
                File.Delete(savedFile);
            }
            catch
            {
                // Best effort cleanup after a failed database write.
            }
        }
    }
}
