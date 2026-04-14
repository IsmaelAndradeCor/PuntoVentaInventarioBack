using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models
{
    public class ProductoDto
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = "";

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "La Descripción es obligatoria.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El Precio de compra es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El Precio de compra debe ser mayor a 0.")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "El Precio de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El Precio de venta debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El Stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El Stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El Stock Minimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El Stock minimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        public string? Categoria { get; set; }

        public string? Proveedor { get; set; }
    }
}
