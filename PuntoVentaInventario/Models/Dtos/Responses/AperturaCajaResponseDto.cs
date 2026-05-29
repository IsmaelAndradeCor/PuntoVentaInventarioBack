namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class AperturaCajaResponseDto
    {
        public int Id { get; set; }
        public DateTime FechaOperacion { get; set; }
        public decimal MontoInicial { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
    }
}