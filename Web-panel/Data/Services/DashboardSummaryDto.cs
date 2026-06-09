namespace Diplom.Data.Services;

public sealed class DashboardSummaryDto
{
    public int TotalLightingObjects { get; set; }
    public int ProblemLightingObjects { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTodayTasks { get; set; }
}
