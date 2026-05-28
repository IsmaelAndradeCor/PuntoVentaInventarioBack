using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class MetodoPago
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public bool AfectaCaja { get; set; } = false;
    }
}