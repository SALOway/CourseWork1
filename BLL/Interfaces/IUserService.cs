using Core.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces;

public interface IUserService
{
    public Result Add(User user);
    public Result<IQueryable<User>> Get(Expression<Func<User, bool>>? predicate = null);
    public Result Update(User user);
    public Result Remove(Expression<Func<User, bool>> predicate);
    public Result<User> Authenticate(string username, string password);
}
