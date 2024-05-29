using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class AnswerOption : BaseEntity
{
    [Required]
    public string Content { get; set; } = null!;

    public bool IsTrue { get; set; } = false;

    [Required]
    public Question Question { get; set; } = null!;

    public IList<UserAnswer> UserAnswers { get; set; } = [];
}
