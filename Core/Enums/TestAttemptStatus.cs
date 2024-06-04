using System.ComponentModel;

namespace Core.Enums;

public enum TestAttemptStatus
{
    [Description("В процесі")]
    InProcess = 0,
    [Description("Завершено")]
    Completed = 1,
    [Description("Прострочено")]
    Expired = 2,
    [Description("Завалено")]
    Failed = 3,
    [Description("Відмінений")]
    Cancelled = 4,
}
