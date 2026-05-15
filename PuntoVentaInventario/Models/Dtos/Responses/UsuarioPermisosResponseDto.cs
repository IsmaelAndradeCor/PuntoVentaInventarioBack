namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class UsuarioPermisosResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public IList<string> Permissions { get; set; } = new List<string>();
    }
}