using System.Globalization;
using System.Security.Claims;
using Diplom.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Diplom.Data.Tasks;

public static class TaskEndpointExtensions
{
    public static RouteGroupBuilder MapTaskApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks")
            .WithTags("Tasks")
            .RequireAuthorization();

        group.MapGet("/my", async Task<Results<Ok<List<MyTaskDto>>, UnauthorizedHttpResult>> (
            ClaimsPrincipal user,
            TaskService taskService) =>
        {
            var userId = GetUserId(user);

            if (userId is null)
                return TypedResults.Unauthorized();

            var tasks = await taskService.GetMyTasksAsync(userId.Value);
            return TypedResults.Ok(tasks);
        });

        group.MapPost("/reports", async (
            ClaimsPrincipal user,
            HttpRequest request,
            TaskReportService reportService) =>
        {
            var userId = GetUserId(user);

            if (userId is null)
                return Results.Unauthorized();

            if (!request.HasFormContentType)
                return Results.BadRequest(new ApiError("Use multipart/form-data."));

            var form = await request.ReadFormAsync();

            if (!TryReadInt(form, "taskId", out var taskId))
                return Results.BadRequest(new ApiError("Field 'taskId' is required."));

            var completionStatus = ReadString(form, "completionStatus", "status");

            if (string.IsNullOrWhiteSpace(completionStatus))
                return Results.BadRequest(new ApiError("Field 'completionStatus' is required."));

            if (!TryReadDateTime(form, out var formedAt))
                return Results.BadRequest(new ApiError("Field 'formedAt' is required."));

            var result = await reportService.CreateReportAsync(new TaskReportCreateDto
            {
                TaskId = taskId,
                CreatedByUserId = userId.Value,
                Comment = ReadString(form, "comment"),
                CompletionStatus = completionStatus,
                FormedAt = formedAt,
                Photos = form.Files.ToList()
            });

            return result.Status switch
            {
                TaskReportCreateStatus.Success => Results.Created(
                    $"/api/tasks/reports/{result.Report!.Id}",
                    result.Report),
                TaskReportCreateStatus.TaskNotFound => Results.NotFound(new ApiError("Task was not found.")),
                TaskReportCreateStatus.Forbidden => Results.Forbid(),
                TaskReportCreateStatus.InvalidPhoto => Results.BadRequest(new ApiError(
                    result.ErrorMessage ?? "Invalid photo.")),
                _ => Results.BadRequest(new ApiError("Report was not created."))
            };
        });

        app.MapGet("/photos/{fileName}", (string fileName, IWebHostEnvironment environment) =>
        {
            var safeFileName = Path.GetFileName(fileName);

            if (!string.Equals(fileName, safeFileName, StringComparison.Ordinal))
                return Results.NotFound();

            var photoPath = Path.Combine(environment.ContentRootPath, "photos", safeFileName);

            if (!File.Exists(photoPath))
                return Results.NotFound();

            return Results.File(photoPath, GetContentType(safeFileName));
        }).RequireAuthorization();

        return group;
    }

    private static int? GetUserId(ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;

    private static bool TryReadInt(IFormCollection form, string key, out int value) =>
        int.TryParse(form[key].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    private static bool TryReadDateTime(IFormCollection form, out DateTime value)
    {
        var formedAt = ReadString(form, "formedAt", "formedDate", "createdAt");

        return DateTime.TryParse(
            formedAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out value);
    }

    private static string ReadString(IFormCollection form, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = form[key].ToString();

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return "";
    }

    private static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            _ => "application/octet-stream"
        };

    private sealed record ApiError(string Message);
}
