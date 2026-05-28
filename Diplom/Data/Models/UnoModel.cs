using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Data.Models;

[Table("UnoModel")]
public class UnoModel
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}