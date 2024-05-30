using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IStudentGroupService
{
    Result Add(StudentGroup studentGroup);
    Result<StudentGroup> GetById(Guid id);
    Result<IQueryable<StudentGroup>> Get(Expression<Func<StudentGroup, bool>>? predicate = null);
    Result Update(StudentGroup studentGroup);
    Result Remove(Expression<Func<StudentGroup, bool>> predicate);
}
