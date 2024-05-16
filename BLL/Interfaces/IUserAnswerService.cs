using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IUserAnswerService
{
    public Result Add(UserAnswer userAnswer);
    public Result<IQueryable<UserAnswer>> Get(Expression<Func<UserAnswer, bool>>? predicate = null);
    public Result Update(UserAnswer userAnswer);
    public Result Remove(Expression<Func<UserAnswer, bool>> predicate);
}
