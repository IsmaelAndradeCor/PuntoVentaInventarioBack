using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class CorteCaja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdAperturaCaja { get; set; }

        [ForeignKey(nameof(IdAperturaCaja))]
        public AperturaCaja AperturaCaja { get; set; } = null!;

        [Required]
        public DateTime FechaCorte { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoInicial { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoVentasEfectivo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoPagoProveedores { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoEsperado { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Retiro { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoFinal { get; set; }

        [Required, MaxLength(450)]
        public string IdUsuarioPrevio { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NombreUsuarioPrevio { get; set; } = string.Empty;

        [Required, MaxLength(450)]
        public string IdUsuarioCorte { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NombreUsuarioCorte { get; set; } = string.Empty;

        [MaxLength(450)]
        public string? IdUsuarioRecepcion { get; set; }

        [MaxLength(200)]
        public string? NombreUsuarioRecepcion { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }
}
