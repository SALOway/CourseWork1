using System.ComponentModel;

namespace Core.Enums;

public enum TestStatus
{
    [Description("В розробці")]
    Draft = 0,
    [Description("Приватний")]
    Private = 1,
    [Description("Публічний")]
    Public = 2,
}
