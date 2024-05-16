using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class AnswerOption : BaseEntity
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = null!;
    public bool IsTrue { get; set; } = false;

    [Required]
    public Question Question { get; set; } = null!;
    public IList<UserAnswer> UserAnswers { get; set; } = [];
}
