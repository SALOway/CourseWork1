using System.Text;

namespace Core.Models;

public class Result(bool isSuccess, List<AppError> errors)
{
    public bool IsSuccess { get; } = isSuccess;
    public string ErrorMessage
    {
        get
        {
            var sb = new StringBuilder();
            foreach (var error in AppErrors)
            {
                sb.AppendLine(error.ToString());
            }
            return sb.ToString();
        }
    }
    public List<AppError> AppErrors { get; } = errors;

    public static Result Success() => new(true, []);
    public static Result Failure(AppError error) => new(false, [error]);
    public static Result Failure(List<AppError> previousErrors)
    {
        return new(false, previousErrors);
    }
    public static Result Failure(AppError error, List<AppError> previousErrors)
    {
        var errors = new List<AppError>(previousErrors) { error };
        return new(false, errors);
    }
}

public class Result<T>(bool isSuccess, List<AppError> errors, T value) : Result(isSuccess, errors)
{
    public T Value { get; } = value;

    public static Result<T> Success(T data) => new(true, [], data);
    public static new Result<T> Failure(AppError error) => new(false, [error], default!);
    public static new Result<T> Failure(List<AppError> previousErrors)
    {
        return new(false, previousErrors, default!);
    }
    public static new Result<T> Failure(AppError error, List<AppError> previousErrors)
    {
        var errors = new List<AppError>(previousErrors) { error };
        return new(false, errors, default!);
    }
}
