namespace Diplom.Data.Services;

public sealed class LightingObjectCreateDto
{
    public string InventoryNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int StatusId { get; set; }
    public string? Description { get; set; }
}
