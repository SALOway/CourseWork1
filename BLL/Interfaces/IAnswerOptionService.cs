using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IAnswerOptionService
{
    Result Add(AnswerOption answerOption);
    Result<IQueryable<AnswerOption>> Get(Expression<Func<AnswerOption, bool>>? predicate = null);
    Result Update(AnswerOption answerOption);
    Result RemoveById(Guid id);
    Result Remove(Expression<Func<AnswerOption, bool>> predicate);
    Result<IQueryable<AnswerOption>> GetAllOptionsForQuestion(Question question);
}
