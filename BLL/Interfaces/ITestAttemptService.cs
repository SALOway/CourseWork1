using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface ITestAttemptService
{
    Result Add(TestAttempt testAttempt);
    Result<IQueryable<TestAttempt>> Get(Expression<Func<TestAttempt, bool>>? predicate = null);
    Result Update(TestAttempt testAttempt);
    Result Remove(Expression<Func<TestAttempt, bool>> predicate);
    Result<TestAttempt?> GetLastAttempt(User user, Test test);
}
