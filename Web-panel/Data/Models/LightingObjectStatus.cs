namespace Diplom.Data.Models;

public class LightingObjectStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<LightingObject> LightingObjects { get; set; } = new List<LightingObject>();
}
