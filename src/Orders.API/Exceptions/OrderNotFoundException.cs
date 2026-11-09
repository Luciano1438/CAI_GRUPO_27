namespace Orders.API.Exceptions;

public class OrderNotFoundException : Exception
{
    public const string ErrorCode = "ORD-001";

    public OrderNotFoundException() : base("Orden no encontrada.")
    {
    }
}