using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface ITestService
{
    Result Add(Test test);
    Result<Test> GetById(Guid id);
    Result<IQueryable<Test>> Get(Expression<Func<Test, bool>>? predicate = null);
    Result Update(Test test);
    Result RemoveById(Guid id);
    Result Remove(Expression<Func<Test, bool>> predicate);
    Result<IQueryable<Test>> GetTestsByGroup(StudentGroup studentGroup, bool onlyPublic = true);
}
