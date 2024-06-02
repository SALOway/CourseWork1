using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class UserRepository(DbContext dbContext) : GenericRepository<User>(dbContext), IGenericRepository<User>
{
    public override Result<IQueryable<User>> Get(Expression<Func<User, bool>>? expression = null)
    {
        try
        {
            IQueryable<User> query = _dbContext.Set<User>()
                .Include(u => u.StudentGroup)
                .Include(u => u.CreatedTests)
                .Include(u => u.TestAttempts)
                .Where(e => !e.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<User>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<User>>.Failure(ex);
        }
    }
}