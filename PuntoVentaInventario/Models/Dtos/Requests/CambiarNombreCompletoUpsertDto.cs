using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class CambiarNombreCompletoUpsertDto
    {
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;
    }
}
