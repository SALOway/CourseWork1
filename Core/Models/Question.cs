using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class Question : BaseEntity
{
    [Required]
    public string Content { get; set; } = null!;
    [EnumDataType(typeof(QuestionType))]
    public QuestionType Type { get; set; } = QuestionType.SingleChoice;
    public int GradeValue { get; set; } = 0;

    [Required]
    public Test Test { get; set; } = null!;
    public IList<AnswerOption> AnswerOptions { get; set; } = [];
    public IList<UserAnswer> UserAnswers { get; set; } = [];
}
