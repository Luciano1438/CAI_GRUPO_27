namespace Orders.API.DTOs;

/// <summary>Datos de Products.API utilizados para validar un producto al crear una orden.</summary>
public class ProductoConsultaResponse
{
    /// <summary>Identificador del producto.</summary>
    public required Guid Id { get; set; }
    /// <summary>Precio actual del producto.</summary>
    public required decimal Precio { get; set; }
    /// <summary>Cantidad de unidades disponibles informada por Products.API.</summary>
    public required int Stock { get; set; }
}