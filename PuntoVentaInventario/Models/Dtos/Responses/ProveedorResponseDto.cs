namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class ProveedorResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
    }
}
