using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class UserAnswerService(IGenericRepository<UserAnswer> userAnswerRepository) : IUserAnswerService
{
    private readonly IGenericRepository<UserAnswer> _repository = userAnswerRepository;

    public Result Add(UserAnswer userAnswer)
    {
        return _repository.Create(userAnswer);
    }
    public Result<UserAnswer> GetById(Guid id)
    {
        var getResult = _repository.Get(u => u.Id == id);
        if (!getResult.IsSuccess)
        {
            return Result<UserAnswer>.Failure(getResult.AppErrors);
        }

        var userAnswer = getResult.Value.FirstOrDefault();
        if (userAnswer == null)
        {
            return Result<UserAnswer>.Failure("Відповіді користувача з даним id не існує");
        }

        return Result<UserAnswer>.Success(userAnswer);
    }
    public Result<IQueryable<UserAnswer>> Get(Expression<Func<UserAnswer, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(UserAnswer userAnswer)
    {
        return _repository.Update(userAnswer);
    }
    public Result Remove(Expression<Func<UserAnswer, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}
