using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface ITestAttemptService
{
    Result Add(TestAttempt testAttempt);
    Result<TestAttempt> GetById(Guid id);
    Result<IQueryable<TestAttempt>> Get(Expression<Func<TestAttempt, bool>>? predicate = null);
    Result Update(TestAttempt testAttempt);
    Result Remove(Expression<Func<TestAttempt, bool>> predicate);
    Result<TestAttempt?> GetLastAttempt(Guid userId, Guid testId);
}
