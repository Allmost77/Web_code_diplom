namespace Diplom.Data.Services;

public sealed class MyTaskDto
{
    public int Id { get; set; }
    public int LightingObjectId { get; set; }
    public string LightingObjectInventoryNumber { get; set; } = "";
    public string LightingObjectAddress { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
}
