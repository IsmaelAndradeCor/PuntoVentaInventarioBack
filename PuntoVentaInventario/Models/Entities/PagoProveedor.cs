using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class PagoProveedor
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Folio { get; set; } = string.Empty;

        [Required]
        public int IdProveedor { get; set; }

        [ForeignKey(nameof(IdProveedor))]
        public Proveedor Proveedor { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required, MaxLength(50)]
        public string MetodoPago { get; set; } = "Efectivo";

        [MaxLength(100)]
        public string? Referencia { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        [Required]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required, MaxLength(450)]
        public string IdUsuario { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;
    }
}