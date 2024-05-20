using BLL.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class UserService(IGenericRepository<User> repository) : IUserService
{
    private readonly IGenericRepository<User> _repository = repository;

    public Result Add(User user)
    {
        if (user.Role == UserRole.Student && user.StudentGroup == null)
        {
            return Result.Failure("A user with the role 'Student' must be associated with a student group");
        }
        else if (user.Role == UserRole.None)
        {
            return Result.Failure("User's role must be specefied");
        }

        return _repository.Create(user);
    }
    public Result<IQueryable<User>> Get(Expression<Func<User, bool>>? predicate = null)
    {
        var getResult = _repository.Get(predicate);
        return getResult.IsSuccess ? Result<IQueryable<User>>.Success(getResult.Value.Where(u => u.Role != UserRole.None)) : getResult;
    }
    public Result Update(User user)
    {
        return _repository.Update(user);
    }
    public Result Remove(Expression<Func<User, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }

    public Result<User> Authenticate(string username, string password)
    {
        var getUserResult = _repository.Get(u => u.Username == username
                                                 && u.Password == password
                                                 && u.Role != UserRole.None);
        if (!getUserResult.IsSuccess)
        {
            return Result<User>.Failure(getUserResult.AppErrors);
        }
        else if (!getUserResult.Value.Any())
        {
            return Result<User>.Failure(new AuthenticationError("User with such username or password don't exist. User wasn't authorized"));
        }

        return Result<User>.Success(getUserResult.Value.First());
    }
}
