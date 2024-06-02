namespace Core.Models;

public class AuthenticationError : AppError
{
    public AuthenticationError(string message) : base(message) { }
    public override string ToString() => Message;
}
