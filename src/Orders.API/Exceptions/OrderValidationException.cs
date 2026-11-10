namespace Orders.API.Exceptions;

public class OrderValidationException : Exception
{
    public const string ErrorCode = "ORD-002";

    public OrderValidationException(string message) : base(message)
    {
    }
}