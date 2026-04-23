using System.ComponentModel.DataAnnotations;

public class ProductoUpsertDto
{
        [Required(ErrorMessage = "El Código es obligatorio.")]
    public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El Costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El Costo debe ser mayor a 0.")]
    public decimal Costo { get; set; }

        [Required(ErrorMessage = "El Precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El Precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

        [Required(ErrorMessage = "El Stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El Stock no puede ser negativo.")]
    public decimal Stock { get; set; }

        [Required(ErrorMessage = "El Stock Minimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El Stock minimo no puede ser negativo.")]
    public decimal StockMinimo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe escribir o seleccionar una Categoria.")]
    public int IdCategoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe escribir o seleccionar una Marca.")]
    public int IdMarca { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe escribir o seleccionar una Unidad Medida.")]
    public int IdUnidadMedida { get; set; }

        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un Proveedor.")]
    public List<int> IdsProveedores { get; set; } = new();
}