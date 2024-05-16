using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class TestRepository(DbContext dbContext) : GenericRepository<Test>(dbContext), IGenericRepository<Test>
{
    public override Result<IQueryable<Test>> Get(Expression<Func<Test, bool>>? expression = null)
    {
        try
        {
            IQueryable<Test> query = _dbContext.Set<Test>()
                .Include(t => t.Questions)
                .Include(t => t.TestAttempts)
                .Where(t => !t.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<Test>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<Test>>.Failure(ex);
        }
    }
}
