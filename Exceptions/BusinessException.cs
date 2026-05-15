namespace nutrition_app_backend.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string code, string message) : base(message)
    {
        Code = code;
    }

    public BusinessException(string code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }
}
