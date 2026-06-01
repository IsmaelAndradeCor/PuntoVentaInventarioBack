using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Folio { get; set; } = string.Empty;

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        [Required, MaxLength(450)]
        public string IdUsuario { get; set; } = string.Empty;

        [Required]
        public int IdMetodoPago { get; set; }

        [ForeignKey(nameof(IdMetodoPago))]
        public MetodoPago MetodoPago { get; set; } = null!;

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
