public class ProductoUpsertDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Costo { get; set; }
    public decimal Precio { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }

    public int? IdCategoria { get; set; }
    public int? IdMarca { get; set; }
    public int? IdUnidadMedida { get; set; }

    public List<int> IdsProveedores { get; set; } = new();
}