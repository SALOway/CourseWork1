using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class StudentGroupRepository(DbContext dbContext) : GenericRepository<StudentGroup>(dbContext), IGenericRepository<StudentGroup>
{
    public override Result<IQueryable<StudentGroup>> Get(Expression<Func<StudentGroup, bool>>? expression = null)
    {
        try
        {
            IQueryable<StudentGroup> query = _dbContext.Set<StudentGroup>()
                .Include(g => g.Students)
                .Include(g => g.Tests)
                .Where(e => !e.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<StudentGroup>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<StudentGroup>>.Failure(ex);
        }
    }
}
