namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class RegistrarPagoProveedorResponseDto
    {
        public int IdPagoProveedor { get; set; }
        public string Folio { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}