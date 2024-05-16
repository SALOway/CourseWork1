using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface ITestAttemptService
{
    public Result Add(TestAttempt testAttempt);
    public Result<IQueryable<TestAttempt>> Get(Expression<Func<TestAttempt, bool>>? predicate = null);
    public Result Update(TestAttempt testAttempt);
    public Result Remove(Expression<Func<TestAttempt, bool>> predicate);
}
