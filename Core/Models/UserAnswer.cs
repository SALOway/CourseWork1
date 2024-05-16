using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class UserAnswer : BaseEntity
{
    public bool IsSelected { get; set; } = false;

    [Required]
    public AnswerOption AnswerOption { get; set; } = null!;
    [Required]
    public TestAttempt TestAttempt { get; set; } = null!;
}