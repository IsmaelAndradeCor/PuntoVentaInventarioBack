namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class MetodosPagoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool AfectaCaja { get; set; }
    }
}