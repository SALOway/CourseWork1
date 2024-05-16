using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class StudentGroup : BaseEntity
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = null!;

    public IList<User> Students { get; set; } = [];
    public IList<Test> Tests { get; set; } = [];
}
