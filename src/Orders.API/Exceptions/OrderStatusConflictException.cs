namespace Orders.API.Exceptions;

public class OrderStatusConflictException : Exception
{
    public const string ErrorCode = "ORD-006";

    public OrderStatusConflictException(string mensaje) : base(mensaje)
    {
    }
}