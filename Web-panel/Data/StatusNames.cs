namespace Diplom.Data;

public static class StatusNames
{
    public static class LightingObject
    {
        public const string Operational = "Исправен";
        public const string Ready = "Готово";
        public const string InProgress = "В работе";
        public const string NeedsReview = "Требует проверки";
        public const string Emergency = "Авария";
    }

    public static class WorkTask
    {
        public const string Completed = "Выполнено";
        public const string InProgress = "В работе";
        public const string New = "Новая";
        public const string Overdue = "Просрочено";
    }
}
