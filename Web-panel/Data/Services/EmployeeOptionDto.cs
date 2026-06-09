namespace Diplom.Data.Services;

public sealed class EmployeeOptionDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string? Email { get; set; }
    public string RoleName { get; set; } = "";
}
