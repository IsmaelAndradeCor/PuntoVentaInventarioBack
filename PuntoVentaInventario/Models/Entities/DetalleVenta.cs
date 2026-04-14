using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Venta")]
        public int IdVenta { get; set; }

        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        [Required, MaxLength(20)]
        public string CodigoProducto { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NombreProducto { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTotal { get; set; }

        public virtual Venta Venta { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
    }
}
