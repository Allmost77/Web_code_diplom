namespace Diplom.Data;

public static class DisplayText
{
    public const string EmptyValue = "-";
    public const string EmployeeLabel = "Сотрудник";

    public static string Employee(int id) => $"{EmployeeLabel} #{id}";

    public static string UserName(int id, string? fullName) =>
        string.IsNullOrWhiteSpace(fullName) ? Employee(id) : fullName;
}
