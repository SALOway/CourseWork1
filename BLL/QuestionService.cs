using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Linq.Expressions;

namespace BLL;

public class QuestionService(IGenericRepository<Question> questionRepository) : IQuestionService
{
    private readonly IGenericRepository<Question> _repository = questionRepository;

    public Result Add(Question question)
    {
        return _repository.Create(question);
    }
    public Result<Question> GetById(Guid id)
    {
        var getResult = _repository.Get(u => u.Id == id);
        if (!getResult.IsSuccess)
        {
            return Result<Question>.Failure(getResult.AppErrors);
        }

        var question = getResult.Value.FirstOrDefault();
        if (question == null)
        {
            return Result<Question>.Failure("Питання з даним id не існує");
        }

        return Result<Question>.Success(question);
    }
    public Result<IQueryable<Question>> Get(Expression<Func<Question, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(Question question)
    {
        return _repository.Update(question);
    }
    public Result RemoveById(Guid id)
    {
        return _repository.Remove(q => q.Id == id);
    }
    public Result Remove(Expression<Func<Question, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}

