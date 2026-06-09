using Diplom.Data;

namespace TestProject1;

public class DisplayTextTests
{
    [Fact(DisplayName = "Имя пользователя возвращается как ФИО, если оно заполнено")]
    public void UserName_ReturnsFullNameWhenItIsProvided()
    {
        var result = DisplayText.UserName(7, "Ivan Petrov");

        Assert.Equal("Ivan Petrov", result);
    }

    [Fact(DisplayName = "Пустое ФИО заменяется подписью сотрудника")]
    public void UserName_ReturnsEmployeeLabelWhenFullNameIsEmpty()
    {
        var result = DisplayText.UserName(7, " ");

        Assert.Equal($"{DisplayText.EmployeeLabel} #7", result);
    }

    [Fact(DisplayName = "Подпись сотрудника содержит его идентификатор")]
    public void Employee_ReturnsEmployeeLabelWithId()
    {
        var result = DisplayText.Employee(12);

        Assert.Equal($"{DisplayText.EmployeeLabel} #12", result);
    }
}
