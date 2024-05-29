using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class TestAttempt : BaseEntity
{
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public bool HasLeftoverTime { get; set; }
    public TimeSpan? LeftoverTime { get; set; }
    [EnumDataType(typeof(TestAttemptStatus))]
    public TestAttemptStatus Status { get; set; } = TestAttemptStatus.InProcess;
    public bool HasGrade { get; set; }
    [Range(0, int.MaxValue)]
    public int? Grade { get; set; }

    [Required]
    public User User { get; set; } = null!;
    [Required]
    public Test Test { get; set; } = null!;
    public IList<UserAnswer> Answers { get; set; } = [];
}
