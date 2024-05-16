using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class TestAttemptService(IGenericRepository<TestAttempt> testAttemptRepository) : ITestAttemptService
{
    private readonly IGenericRepository<TestAttempt> _repository = testAttemptRepository;

    public Result Add(TestAttempt testAttempt)
    {
        return _repository.Create(testAttempt);
    }
    public Result<IQueryable<TestAttempt>> Get(Expression<Func<TestAttempt, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(TestAttempt testAttempt)
    {
        return _repository.Update(testAttempt);
    }
    public Result Remove(Expression<Func<TestAttempt, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}