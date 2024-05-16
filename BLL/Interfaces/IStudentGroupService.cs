using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IStudentGroupService
{
    public Result Add(StudentGroup studentGroup);
    public Result<IQueryable<StudentGroup>> Get(Expression<Func<StudentGroup, bool>>? predicate = null);
    public Result Update(StudentGroup studentGroup);
    public Result Remove(Expression<Func<StudentGroup, bool>> predicate);
}
