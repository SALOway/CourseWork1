using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IUserAnswerService
{
    Result Add(UserAnswer userAnswer);
    Result<IQueryable<UserAnswer>> Get(Expression<Func<UserAnswer, bool>>? predicate = null);
    Result Update(UserAnswer userAnswer);
    Result Remove(Expression<Func<UserAnswer, bool>> predicate);
}
