using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class User : BaseEntity
{
    [Required]
    [MinLength(1)]
    public string Username { get; set; } = null!;
    [Required]
    [MinLength(1)]
    public string Password { get; set; } = null!;
    [Required]
    [MinLength(1)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MinLength(1)]
    public string LastName { get; set; } = null!;
    [EnumDataType(typeof(UserRole))]
    public UserRole Role { get; set; } = UserRole.None;

    public StudentGroup? StudentGroup { get; set; }
    public IList<Test> CreatedTests { get; set; } = [];
    public IList<TestAttempt> TestAttempts { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
