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
        var getTestAttempts = _repository.Get(predicate);
        if (!getTestAttempts.IsSuccess)
        {
            return Result<IQueryable<TestAttempt>>.Failure(getTestAttempts.AppErrors);
        }

        var query = getTestAttempts.Value;
        
        return Result<IQueryable<TestAttempt>>.Success(query.OrderBy(a => a.CreatedAt));
    }
    public Result Update(TestAttempt testAttempt)
    {
        return _repository.Update(testAttempt);
    }
    public Result Remove(Expression<Func<TestAttempt, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }

    public Result<TestAttempt?> GetLastAttempt(Guid userId, Guid testId)
    {
        var getAttempts = Get(a => a.Test.Id == testId && a.User.Id == userId);
        if (!getAttempts.IsSuccess)
        {
            return Result<TestAttempt?>.Failure(getAttempts.AppErrors);
        }

        var lastAttempt = getAttempts.Value.OrderByDescending(a => a.StartedAt).FirstOrDefault();

        return Result<TestAttempt?>.Success(lastAttempt);
    }
}