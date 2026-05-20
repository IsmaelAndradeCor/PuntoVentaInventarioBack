using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class Folio
    {
        [Key]
        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int UltimoNumero { get; set; }
    }
}   