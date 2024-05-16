using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IAnswerOptionService
{
    public Result Add(AnswerOption answerOption);
    public Result<IQueryable<AnswerOption>> Get(Expression<Func<AnswerOption, bool>>? predicate = null);
    public Result Update(AnswerOption answerOption);
    public Result Remove(Expression<Func<AnswerOption, bool>> predicate);
}
