using System.Diagnostics;
using System.Reflection;

namespace Core.Models;

public class AppError
{
    public AppError()
    {
        var stackTrace = new StackTrace();
        
        var frames = stackTrace.GetFrames();

        var stackFrame = frames.FirstOrDefault(f => f?.GetMethod()?.DeclaringType?.Name != typeof(AppError).Name);

        if (stackFrame != null)
        {
            Source = stackFrame.GetMethod();
        }
    }
    public AppError(string errorMessage) : this()
    {
        Message = errorMessage;
    }
    public AppError(Exception exception) : this(exception.Message)
    {
        Exception = exception;
        while (exception.InnerException != null)
        {
            Message += $"\n{exception.InnerException.Message}";
            exception = exception.InnerException;
        }
    }

    public MethodBase? Source { get; set; }
    public string Message { get; set; } = "There was some error";
    public Exception? Exception { get; set; }

    public override string ToString()
    {
        string message;
        List<string> sourceInfo = [];

        if (Source != null)
        {
            var type = Source.DeclaringType;
            var typeNamespace = type?.Namespace;
            var namespaceParts = typeNamespace?.Split('.');
            if (namespaceParts != null && namespaceParts.Length != 0)
            {
                sourceInfo.Add(namespaceParts[0]);
            }
            else
            {
                sourceInfo.Add("UndefinedNamespace");
            }

            var typeName = type?.Name;
            if (typeName != null)
            {
                sourceInfo.Add(typeName);
            }
            else
            {
                sourceInfo.Add("UndefinedClass");
            }

            var methodName = Source.Name;
            if (methodName != null)
            {
                sourceInfo.Add(methodName);
            }
            else
            {
                sourceInfo.Add("UndefinedMethod");
            }

            message = $"[{string.Join(':', sourceInfo)}] {Message}";
        }
        else 
        {
            message = $"[UndefinedSource] {Message}";
        }

        return message;
    }

    public static implicit operator AppError(string message) => new(message);
    public static implicit operator AppError(Exception exception) => new(exception);
}
