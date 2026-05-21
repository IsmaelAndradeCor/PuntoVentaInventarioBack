namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class PermisoNodoDto
    {
        public string Key { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Permission { get; set; }
        public List<PermisoNodoDto> Hijos { get; set; } = [];
    }
}