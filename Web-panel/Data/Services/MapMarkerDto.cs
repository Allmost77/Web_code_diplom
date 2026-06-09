namespace Diplom.Data.Services;

public sealed class MapMarkerDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
