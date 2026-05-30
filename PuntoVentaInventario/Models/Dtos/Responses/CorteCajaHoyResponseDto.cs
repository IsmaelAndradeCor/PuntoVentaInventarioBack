namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class CorteCajaHoyResponseDto
    {
        public DateTime FechaOperacion { get; set; }
        public decimal MontoInicialCaja { get; set; }
        public decimal MontoVentas { get; set; }
        public decimal MontoPagoProveedores { get; set; }
        public decimal CorteCaja { get; set; }
    }
}