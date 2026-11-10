namespace Orders.API.DTOs;

public class ProductoConsultaResponse
{
    public required Guid Id { get; set; }
    public required decimal Precio { get; set; }
    public required int Stock { get; set; }
}