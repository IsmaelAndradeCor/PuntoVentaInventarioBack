using Microsoft.AspNetCore.Identity;

namespace PuntoVentaInventario.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
