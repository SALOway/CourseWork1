using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IQuestionService
{
    public Result Add(Question question);
    public Result<IQueryable<Question>> Get(Expression<Func<Question, bool>>? predicate = null);
    public Result Update(Question question);
    public Result Remove(Expression<Func<Question, bool>> predicate);
}
