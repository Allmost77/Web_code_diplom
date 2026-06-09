using System.Text;
using Diplom.Data;
using Diplom.Data.DbContext;
using Diplom.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data.Services;

public sealed class TaskService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<List<StatusOptionDto>> GetTaskStatusesAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.TaskStatuses
            .AsNoTracking()
            .OrderBy(status => status.Id)
            .Select(status => new StatusOptionDto
            {
                Id = status.Id,
                Name = status.Name
            })
            .ToListAsync();
    }

    public async Task<List<EmployeeOptionDto>> GetEmployeesAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var users = await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .Select(user => new
            {
                user.Id,
                user.FullName,
                user.Email,
                RoleName = user.Role != null ? user.Role.Name : ""
            })
            .ToListAsync();

        return users
            .Select(user => new EmployeeOptionDto
            {
                Id = user.Id,
                DisplayName = DisplayText.UserName(user.Id, user.FullName),
                Email = user.Email,
                RoleName = user.RoleName
            })
            .ToList();
    }

    public async Task<int> CreateTaskAsync(TaskCreateDto createDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var task = new TaskItem
        {
            LightingObjectId = createDto.LightingObjectId,
            CreatedByUserId = createDto.CreatedByUserId,
            AssignedToUserId = createDto.AssignedToUserId,
            StatusId = createDto.StatusId,
            Title = $"{createDto.TaskType} / {createDto.Priority}",
            Description = BuildDescription(createDto),
            CreatedAt = ToDatabaseTimestamp(DateTime.Now),
            Deadline = ToDatabaseTimestamp(createDto.Deadline)
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        return task.Id;
    }

    public async Task<List<MyTaskDto>> GetMyTasksAsync(int userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.AssignedToUserId == userId)
            .OrderBy(task => task.Deadline == null)
            .ThenBy(task => task.Deadline)
            .ThenByDescending(task => task.CreatedAt)
            .Select(task => new MyTaskDto
            {
                Id = task.Id,
                LightingObjectId = task.LightingObjectId,
                LightingObjectInventoryNumber = task.LightingObject.InventoryNumber,
                LightingObjectAddress = task.LightingObject.Address,
                Latitude = (double)task.LightingObject.Latitude,
                Longitude = (double)task.LightingObject.Longitude,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.Name,
                CreatedAt = task.CreatedAt,
                Deadline = task.Deadline
            })
            .ToListAsync();
    }

    public async Task<TaskDetailsDto?> GetTaskDetailsAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id)
            .Select(task => new TaskDetailsDto
            {
                Id = task.Id,
                LightingObjectId = task.LightingObjectId,
                LightingObjectInventoryNumber = task.LightingObject.InventoryNumber,
                LightingObjectAddress = task.LightingObject.Address,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.Name,
                CreatedByUserId = task.CreatedByUserId,
                AssignedToUserId = task.AssignedToUserId,
                Executor = task.AssignedToUserId.HasValue
                    ? string.IsNullOrWhiteSpace(task.AssignedToUser!.FullName)
                        ? $"{DisplayText.EmployeeLabel} #{task.AssignedToUserId.Value}"
                        : task.AssignedToUser.FullName
                    : DisplayText.EmptyValue,
                CreatedByUser = string.IsNullOrWhiteSpace(task.CreatedByUser.FullName)
                    ? $"{DisplayText.EmployeeLabel} #{task.CreatedByUserId}"
                    : task.CreatedByUser.FullName,
                CreatedAt = task.CreatedAt,
                Deadline = task.Deadline
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TaskEditDto?> GetTaskForEditAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id)
            .Select(task => new TaskEditDto
            {
                Id = task.Id,
                LightingObjectId = task.LightingObjectId,
                Title = task.Title,
                Description = task.Description,
                StatusId = task.StatusId,
                AssignedToUserId = task.AssignedToUserId,
                Deadline = task.Deadline
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateTaskAsync(TaskEditDto editDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var task = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == editDto.Id);

        if (task is null)
            return false;

        task.Title = editDto.Title.Trim();
        task.Description = TrimToNull(editDto.Description);
        task.StatusId = editDto.StatusId;
        task.AssignedToUserId = editDto.AssignedToUserId;
        task.Deadline = ToDatabaseTimestamp(editDto.Deadline);

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static string BuildDescription(TaskCreateDto createDto)
    {
        var description = new StringBuilder();

        description.AppendLine($"Тип задачи: {createDto.TaskType}");
        description.AppendLine($"Приоритет: {createDto.Priority}");
        description.AppendLine();
        description.AppendLine("Описание проблемы:");
        description.AppendLine(createDto.ProblemDescription.Trim());

        if (!string.IsNullOrWhiteSpace(createDto.OperatorComment))
        {
            description.AppendLine();
            description.AppendLine("Комментарий оператора:");
            description.AppendLine(createDto.OperatorComment.Trim());
        }

        return description.ToString();
    }

    private static DateTime ToDatabaseTimestamp(DateTime value) =>
        DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

    private static DateTime? ToDatabaseTimestamp(DateTime? value) =>
        value.HasValue ? ToDatabaseTimestamp(value.Value) : null;

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
