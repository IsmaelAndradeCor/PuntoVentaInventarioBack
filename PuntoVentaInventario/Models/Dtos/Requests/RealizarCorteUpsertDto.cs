using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RealizarCorteUpsertDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El retiro no puede ser negativo.")]
        public decimal Retiro { get; set; }

        public string? IdUsuarioRecepcion { get; set; }

        public string? Observaciones { get; set; }
    }
}
