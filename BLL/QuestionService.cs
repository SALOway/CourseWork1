using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class QuestionService(IGenericRepository<Question> questionRepository) : IQuestionService
{
    private readonly IGenericRepository<Question> _repository = questionRepository;

    public Result Add(Question question)
    {
        return _repository.Create(question);
    }
    public Result<IQueryable<Question>> Get(Expression<Func<Question, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(Question question)
    {
        return _repository.Update(question);
    }
    public Result Remove(Expression<Func<Question, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}

