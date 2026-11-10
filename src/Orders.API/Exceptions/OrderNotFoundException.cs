namespace Orders.API.Exceptions;

public class OrderNotFoundException : Exception
{
    public const string ErrorCode = "ORD-001";

    public string Codigo { get; }

    public OrderNotFoundException()
        : this(ErrorCode, "Orden no encontrada.")
    {
    }

    public OrderNotFoundException(string codigo, string mensaje)
        : base(mensaje)
    {
        Codigo = codigo;
    }
}