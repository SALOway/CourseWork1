using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class User : BaseEntity
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [EnumDataType(typeof(UserRole))]
    public UserRole Role { get; set; } = UserRole.None;

    public bool HasGroup => Role == UserRole.Student;

    public StudentGroup? StudentGroup { get; set; }

    public IList<Test> CreatedTests { get; set; } = [];

    public IList<TestAttempt> TestAttempts { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
