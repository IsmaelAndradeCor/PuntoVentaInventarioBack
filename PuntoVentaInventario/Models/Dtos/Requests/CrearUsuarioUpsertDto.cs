using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class CrearUsuarioUpsertDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;
        [Required]
        public string Rol { get; set; } = "Empleado";
    }
}
