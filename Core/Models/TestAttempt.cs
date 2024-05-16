using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class TestAttempt : BaseEntity
{
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    [EnumDataType(typeof(TestAttemptStatus))]
    public TestAttemptStatus Status { get; set; } = TestAttemptStatus.InProcess;

    [Required]
    public User User { get; set; } = null!;
    [Required]
    public Test Test { get; set; } = null!;
    public IList<UserAnswer> Answers { get; set; } = [];
}
