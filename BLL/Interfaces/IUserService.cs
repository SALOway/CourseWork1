using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IUserService
{
    Result Add(User user);
    Result<IQueryable<User>> Get(Expression<Func<User, bool>>? predicate = null);
    Result<User> GetById(Guid id);
    Result Update(User user);
    Result Remove(Expression<Func<User, bool>> predicate);
    Result<User> Authenticate(string username, string password);
}
