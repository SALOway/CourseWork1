using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class TestService(IGenericRepository<Test> testRepository) : ITestService
{
    private readonly IGenericRepository<Test> _repository = testRepository;

    public Result Add(Test test)
    {
        return _repository.Create(test);
    }
    public Result<IQueryable<Test>> Get(Expression<Func<Test, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(Test test)
    {
        return _repository.Update(test);
    }
    public Result Remove(Expression<Func<Test, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}
