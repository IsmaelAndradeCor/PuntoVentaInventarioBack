using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class CambiarPasswordUpsertDto
    {
        [Required]
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
