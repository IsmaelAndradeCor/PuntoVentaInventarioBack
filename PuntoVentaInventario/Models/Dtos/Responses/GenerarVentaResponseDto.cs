namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class GenerarVentaResponseDto
    {
        public int IdVenta { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Ganancias { get; set; }
        public int IdMetodoPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public List<GenerarVentaDetalleResponseDto> Detalles { get; set; } = new();
    }
}
