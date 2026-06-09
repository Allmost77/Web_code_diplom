namespace Diplom.Data.Services;

public sealed class LightingObjectDto
{
    public int Id { get; set; }
    public string InventoryNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
    public string Assignee { get; set; } = "-";
    public DateTime? UpdatedAt { get; set; }
}
