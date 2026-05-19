using PuntoVentaInventario.Models.Dtos.Responses;

public class ProductoSimpleResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Costo { get; set; }
    public decimal Precio { get; set; }
    public decimal Stock { get; set; }
    public UnidadMedidaResponseDto? UnidadMedida { get; set; }

}