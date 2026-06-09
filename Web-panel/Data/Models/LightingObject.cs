namespace Diplom.Data.Models;

public class LightingObject
{
    public int Id { get; set; }
    public string InventoryNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int StatusId { get; set; }
    public string? Description { get; set; }

    public LightingObjectStatus Status { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
