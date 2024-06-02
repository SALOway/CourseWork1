using Core.Models;
using System.Linq.Expressions;

namespace DAL.Repositories.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity, new()
{
    Result Create(T entity);
    Result<IQueryable<T>> Get(Expression<Func<T, bool>>? expression = null);
    Result Update(T entity);
    Result Remove(Expression<Func<T, bool>> expression);
}
