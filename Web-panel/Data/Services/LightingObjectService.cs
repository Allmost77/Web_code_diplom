using Diplom.Data;
using Diplom.Data.DbContext;
using Diplom.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data.Services;

public sealed class LightingObjectService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var today = DateOnly.FromDateTime(DateTime.Today).ToDateTime(TimeOnly.MinValue);

        return new DashboardSummaryDto
        {
            TotalLightingObjects = await dbContext.LightingObjects.CountAsync(),
            ProblemLightingObjects = await dbContext.LightingObjects
                .CountAsync(lightingObject => lightingObject.Status.Name != StatusNames.LightingObject.Operational),
            ActiveTasks = await dbContext.Tasks
                .CountAsync(task => task.Status.Name != StatusNames.WorkTask.Completed),
            CompletedTodayTasks = await dbContext.Tasks
                .CountAsync(task => task.Status.Name == StatusNames.WorkTask.Completed && task.CreatedAt >= today)
        };
    }

    public async Task<List<LightingObjectDto>> GetLatestLightingObjectsAsync(int take = 10)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.LightingObjects
            .AsNoTracking()
            .OrderByDescending(lightingObject => lightingObject.Id)
            .Take(take)
            .Select(lightingObject => new LightingObjectDto
            {
                Id = lightingObject.Id,
                InventoryNumber = lightingObject.InventoryNumber,
                Address = lightingObject.Address,
                Status = lightingObject.Status.Name,
                UpdatedAt = lightingObject.Tasks
                    .OrderByDescending(task => task.CreatedAt)
                    .Select(task => (DateTime?)task.CreatedAt)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<List<MapMarkerDto>> GetMapMarkersAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.LightingObjects
            .AsNoTracking()
            .Select(lightingObject => new MapMarkerDto
            {
                Id = lightingObject.Id,
                Title = lightingObject.InventoryNumber,
                Address = lightingObject.Address,
                Status = lightingObject.Status.Name,
                Latitude = (double)lightingObject.Latitude,
                Longitude = (double)lightingObject.Longitude
            })
            .ToListAsync();
    }

    public async Task<LightingObjectDetailsDto?> GetLightingObjectDetailsAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.LightingObjects
            .AsNoTracking()
            .Where(lightingObject => lightingObject.Id == id)
            .Select(lightingObject => new LightingObjectDetailsDto
            {
                Id = lightingObject.Id,
                InventoryNumber = lightingObject.InventoryNumber,
                Address = lightingObject.Address,
                Status = lightingObject.Status.Name,
                Latitude = lightingObject.Latitude,
                Longitude = lightingObject.Longitude,
                Description = lightingObject.Description,
                TasksCount = lightingObject.Tasks.Count,
                ActiveTasksCount = lightingObject.Tasks.Count(
                    task => task.Status.Name != StatusNames.WorkTask.Completed),
                LastTaskCreatedAt = lightingObject.Tasks
                    .OrderByDescending(task => task.CreatedAt)
                    .Select(task => (DateTime?)task.CreatedAt)
                    .FirstOrDefault(),
                Tasks = lightingObject.Tasks
                    .OrderByDescending(task => task.CreatedAt)
                    .Select(task => new TaskListItemDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Status = task.Status.Name,
                        Executor = task.AssignedToUserId.HasValue
                            ? string.IsNullOrWhiteSpace(task.AssignedToUser!.FullName)
                                ? $"{DisplayText.EmployeeLabel} #{task.AssignedToUserId.Value}"
                                : task.AssignedToUser.FullName
                            : DisplayText.EmptyValue,
                        CreatedAt = task.CreatedAt,
                        Deadline = task.Deadline
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<StatusOptionDto>> GetLightingObjectStatusesAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.LightingObjectStatuses
            .AsNoTracking()
            .OrderBy(status => status.Name)
            .Select(status => new StatusOptionDto
            {
                Id = status.Id,
                Name = status.Name
            })
            .ToListAsync();
    }

    public async Task<int> CreateLightingObjectAsync(LightingObjectCreateDto createDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var lightingObject = new LightingObject
        {
            InventoryNumber = createDto.InventoryNumber.Trim(),
            Address = createDto.Address.Trim(),
            Latitude = createDto.Latitude,
            Longitude = createDto.Longitude,
            StatusId = createDto.StatusId,
            Description = TrimToNull(createDto.Description)
        };

        dbContext.LightingObjects.Add(lightingObject);
        await dbContext.SaveChangesAsync();

        return lightingObject.Id;
    }

    public async Task<LightingObjectEditDto?> GetLightingObjectForEditAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.LightingObjects
            .AsNoTracking()
            .Where(lightingObject => lightingObject.Id == id)
            .Select(lightingObject => new LightingObjectEditDto
            {
                Id = lightingObject.Id,
                InventoryNumber = lightingObject.InventoryNumber,
                Address = lightingObject.Address,
                Latitude = lightingObject.Latitude,
                Longitude = lightingObject.Longitude,
                StatusId = lightingObject.StatusId,
                Description = lightingObject.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateLightingObjectAsync(LightingObjectEditDto editDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var lightingObject = await dbContext.LightingObjects
            .FirstOrDefaultAsync(lightingObject => lightingObject.Id == editDto.Id);

        if (lightingObject is null)
            return false;

        lightingObject.InventoryNumber = editDto.InventoryNumber.Trim();
        lightingObject.Address = editDto.Address.Trim();
        lightingObject.Latitude = editDto.Latitude;
        lightingObject.Longitude = editDto.Longitude;
        lightingObject.StatusId = editDto.StatusId;
        lightingObject.Description = TrimToNull(editDto.Description);

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLightingObjectAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var lightingObject = await dbContext.LightingObjects
            .FirstOrDefaultAsync(lightingObject => lightingObject.Id == id);

        if (lightingObject is null)
            return false;

        dbContext.LightingObjects.Remove(lightingObject);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
