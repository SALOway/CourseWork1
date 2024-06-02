using System.ComponentModel;

namespace Core.Enums;

public enum QuestionType
{
    [Description("Питання з одним варіантом відповіді")]
    SingleChoice = 0,
    [Description("Питання з декількома варіантами відповіді")]
    MultipleChoice = 1,
}
