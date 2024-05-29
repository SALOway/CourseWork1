using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class AnswerOptionService(IGenericRepository<AnswerOption> answerOptionRepository) : IAnswerOptionService
{
    private readonly IGenericRepository<AnswerOption> _repository = answerOptionRepository;

    public Result Add(AnswerOption answerOption)
    {
        return _repository.Create(answerOption);
    }
    public Result<IQueryable<AnswerOption>> Get(Expression<Func<AnswerOption, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(AnswerOption answerOption)
    {
        return _repository.Update(answerOption);
    }
    public Result Remove(Expression<Func<AnswerOption, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }

    public Result<IQueryable<AnswerOption>> GetAllOptionsForQuestion(Question question)
    {
        return Get(o => o.Question.Id == question.Id);
    }
}
