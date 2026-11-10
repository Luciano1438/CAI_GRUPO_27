namespace Orders.API.Exceptions;

public class InsufficientStockException : Exception
{
    public const string ErrorCode = "ORD-005";

    public InsufficientStockException(Guid productoId)
        : base($"Stock insuficiente para el producto {productoId}.")
    {
    }
}