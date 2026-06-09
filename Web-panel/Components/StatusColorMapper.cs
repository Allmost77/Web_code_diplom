using Diplom.Data;
using MudBlazor;

namespace Diplom.Components;

public static class StatusColorMapper
{
    public static Color ForLightingObject(string status) =>
        status switch
        {
            StatusNames.LightingObject.Operational => Color.Success,
            StatusNames.LightingObject.Ready => Color.Success,
            StatusNames.LightingObject.InProgress => Color.Info,
            StatusNames.LightingObject.NeedsReview => Color.Warning,
            StatusNames.LightingObject.Emergency => Color.Error,
            _ => Color.Default
        };

    public static Color ForWorkTask(string status) =>
        status switch
        {
            StatusNames.WorkTask.Completed => Color.Success,
            StatusNames.WorkTask.InProgress => Color.Info,
            StatusNames.WorkTask.New => Color.Primary,
            StatusNames.WorkTask.Overdue => Color.Error,
            _ => Color.Default
        };
}
