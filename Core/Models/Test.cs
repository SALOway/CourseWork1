using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class Test : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;

    public bool HasDesription { get; set; } = false;

    public string? Description { get; set; }

    [Required]
    [EnumDataType(typeof(TestStatus))]
    public TestStatus Status { get; set; } = TestStatus.Draft;

    public bool HasAttempts { get; set; } = false;

    [Range(1, int.MaxValue)]
    public int? MaxAttempts { get; set; }

    public bool HasRequiredGrade { get; set; } = false;

    [Range(1, int.MaxValue)]
    public int? RequiredGrade { get; set; }

    public bool HasTermin { get; set; } = false;

    public DateTime? Termin { get; set; }

    public bool HasTimeLimit { get; set; } = false;
    public DateTime? TimeLimit { get; set; }

    [Required]
    public StudentGroup StudentGroup { get; set; } = null!;

    [Required]
    public User Creator { get; set; } = null!;

    public IList<Question> Questions { get; set; } = [];

    public IList<TestAttempt> TestAttempts { get; set; } = [];
}
