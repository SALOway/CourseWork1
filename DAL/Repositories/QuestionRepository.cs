using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class QuestionRepository(DbContext dbContext) : GenericRepository<Question>(dbContext), IGenericRepository<Question>
{
    public override Result<IQueryable<Question>> Get(Expression<Func<Question, bool>>? expression = null)
    {
        try
        {
            IQueryable<Question> query = _dbContext.Set<Question>()
                .Include(q => q.UserAnswers)
                .Include(q => q.AnswerOptions)
                .Where(q => !q.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<Question>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<Question>>.Failure(ex);
        }
    }
}
