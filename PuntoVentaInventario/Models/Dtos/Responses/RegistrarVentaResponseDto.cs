namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class RegistrarVentaResponseDto
    {
        public int IdVenta { get; set; }
        public string Folio { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
