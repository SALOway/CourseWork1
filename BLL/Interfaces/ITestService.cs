using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface ITestService
{
    public Result Add(Test test);
    public Result<IQueryable<Test>> Get(Expression<Func<Test, bool>>? predicate = null);
    public Result Update(Test test);
    public Result Remove(Expression<Func<Test, bool>> predicate);
}
