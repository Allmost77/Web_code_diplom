using Diplom.Components;
using Diplom.Data;
using MudBlazor;

namespace TestProject1;

public class StatusColorMapperTests
{
    [Fact(DisplayName = "Статусы преобразуются в ожидаемые цвета")]
    public void KnownAndUnknownStatuses_ReturnExpectedColors()
    {
        Assert.Equal(Color.Error, StatusColorMapper.ForLightingObject(StatusNames.LightingObject.Emergency));
        Assert.Equal(Color.Success, StatusColorMapper.ForWorkTask(StatusNames.WorkTask.Completed));
        Assert.Equal(Color.Default, StatusColorMapper.ForLightingObject("Unknown"));
        Assert.Equal(Color.Default, StatusColorMapper.ForWorkTask("Unknown"));
    }

    [Fact(DisplayName = "Статус задачи в работе отображается информационным цветом")]
    public void ForWorkTaskInProgress_ReturnsInfo()
    {
        var color = StatusColorMapper.ForWorkTask(StatusNames.WorkTask.InProgress);

        Assert.Equal(Color.Info, color);
    }

    [Fact(DisplayName = "Исправное УНО отображается успешным цветом")]
    public void ForLightingObjectOperational_ReturnsSuccess()
    {
        var color = StatusColorMapper.ForLightingObject(StatusNames.LightingObject.Operational);

        Assert.Equal(Color.Success, color);
    }
}
