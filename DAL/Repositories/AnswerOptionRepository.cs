using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class AnswerOptionRepository(DbContext dbContext) : GenericRepository<AnswerOption>(dbContext), IGenericRepository<AnswerOption>
{
    public override Result<IQueryable<AnswerOption>> Get(Expression<Func<AnswerOption, bool>>? expression = null)
    {
        try
        {
            IQueryable<AnswerOption> query = _dbContext.Set<AnswerOption>()
                .Include(o => o.UserAnswers)
                .Include(o => o.Question)
                .Where(o => !o.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<AnswerOption>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<AnswerOption>>.Failure(ex);
        }
    }
}