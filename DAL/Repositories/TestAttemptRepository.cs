using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class TestAttemptRepository(DbContext dbContext) : GenericRepository<TestAttempt>(dbContext), IGenericRepository<TestAttempt>
{
    public override Result<IQueryable<TestAttempt>> Get(Expression<Func<TestAttempt, bool>>? expression = null)
    {
        try
        {
            IQueryable<TestAttempt> query = _dbContext.Set<TestAttempt>()
                .Include(a => a.Answers)
                .Where(a => !a.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<TestAttempt>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<TestAttempt>>.Failure(ex);
        }
    }
}