using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class UnidadMedidaUpsertDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Clave { get; set; } = string.Empty;
        [Required]
        public bool PermiteDecimales { get; set; }
    }
}
