using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IQuestionService
{
    Result Add(Question question);
    Result<Question> GetById(Guid id);
    Result<IQueryable<Question>> Get(Expression<Func<Question, bool>>? predicate = null);
    Result Update(Question question);
    Result RemoveById(Guid id);
    Result Remove(Expression<Func<Question, bool>> predicate);
}
