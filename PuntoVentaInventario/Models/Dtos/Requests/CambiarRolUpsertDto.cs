using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class CambiarRolUpsertDto
    {
        [Required]
        public string Rol { get; set; } = string.Empty;
    }
}
