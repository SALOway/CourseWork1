using Core.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public class GenericRepository<T>(DbContext dbContext) : IGenericRepository<T> where T : BaseEntity, new()
{
    protected readonly DbContext _dbContext = dbContext;

    public virtual Result Create(T entity)
    {
        try
        {
            _dbContext.Add(entity);
            _dbContext.SaveChanges();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            return Result.Failure(ex);
        }
    }
    public virtual Result<IQueryable<T>> Get(Expression<Func<T, bool>>? expression = null)
    {
        try
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(e => !e.IsDeleted);
            if (expression != null)
            {
                query = query.Where(expression);
            }

            return Result<IQueryable<T>>.Success(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<T>>.Failure(ex);
        }
    }
    public virtual Result Update(T entity)
    {
        try
        {
            if (!_dbContext.Set<T>().Any(e => e.Id == entity.Id))
            {
                return Result.Failure("Entity doesn't exist");
            }

            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;

            _dbContext.Update(entity);
            _dbContext.SaveChanges();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            return Result.Failure(ex);
        }
    }
    public virtual Result Remove(Expression<Func<T, bool>> expression)
    {
        try
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(expression).Where(e => !e.IsDeleted);

            if (query.FirstOrDefault() == null)
            {
                return Result.Success();
            }

            foreach (var item in query)
            {
                _dbContext.RemoveRange(item);
            }

            _dbContext.SaveChanges();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            return Result.Failure(ex);
        }
    }
}
