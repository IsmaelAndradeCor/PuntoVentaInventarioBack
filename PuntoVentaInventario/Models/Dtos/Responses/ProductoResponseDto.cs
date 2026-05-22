using PuntoVentaInventario.Models.Dtos.Responses;

public class ProductoResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Costo { get; set; }
    public decimal Precio { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }

    public CategoriaResponseDto? Categoria { get; set; }

    public MarcaResponseDto? Marca { get; set; }

    public UnidadMedidaResponseDto? UnidadMedida { get; set; }

    public List<ProveedorResponseDto> Proveedores { get; set; } = new();
}