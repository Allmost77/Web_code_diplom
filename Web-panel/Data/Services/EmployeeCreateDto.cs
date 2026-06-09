namespace Diplom.Data.Services;

public sealed class EmployeeCreateDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int RoleId { get; set; }
}
