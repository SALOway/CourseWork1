using BLL.Interfaces;
using Core.Models;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BLL;

public class StudentGroupService(IGenericRepository<StudentGroup> groupRepository) : IStudentGroupService
{
    private readonly IGenericRepository<StudentGroup> _repository = groupRepository;

    public Result Add(StudentGroup studentGroup)
    {
        return _repository.Create(studentGroup);
    }
    public Result<StudentGroup> GetById(Guid id)
    {
        var getResult = _repository.Get(u => u.Id == id);
        if (!getResult.IsSuccess)
        {
            return Result<StudentGroup>.Failure(getResult.AppErrors);
        }

        var group = getResult.Value.FirstOrDefault();
        if (group == null)
        {
            return Result<StudentGroup>.Failure("Групи з даним id не існує");
        }

        return Result<StudentGroup>.Success(group);
    }
    public Result<IQueryable<StudentGroup>> Get(Expression<Func<StudentGroup, bool>>? predicate = null)
    {
        return _repository.Get(predicate);
    }
    public Result Update(StudentGroup studentGroup)
    {
        return _repository.Update(studentGroup);
    }
    public Result Remove(Expression<Func<StudentGroup, bool>> predicate)
    {
        return _repository.Remove(predicate);
    }
}
