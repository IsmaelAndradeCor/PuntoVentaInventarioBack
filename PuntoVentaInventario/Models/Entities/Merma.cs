using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class Merma
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Folio { get; set; } = string.Empty;

        public DateTime FechaMerma { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoTotal { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        [Required, MaxLength(450)]
        public string IdUsuario { get; set; } = string.Empty;

        public ICollection<DetalleMerma> Detalles { get; set; } = new List<DetalleMerma>();
    }
}
