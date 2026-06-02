using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class AperturaCaja
    {
        public int Id { get; set; }

        [Required]
        public DateTime FechaOperacion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoInicial { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(450)]
        public string IdUsuario { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;
    }
}