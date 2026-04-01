using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models
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

        public int IdUsuario { get; set; }

        [Required, MaxLength(20)]
        public string FormaPago { get; set; } = "Efectivo";

        //public bool Activo { get; set; } = true;

        //public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        //public DateTime? FechaModificacion { get; set; }
        //public DateTime? FechaEliminacion { get; set; }

        //public int IdUsuarioCreacion { get; set; }
        //public int? IdUsuarioModificacion { get; set; }
        //public int? IdUsuarioEliminacion { get; set; }

        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
