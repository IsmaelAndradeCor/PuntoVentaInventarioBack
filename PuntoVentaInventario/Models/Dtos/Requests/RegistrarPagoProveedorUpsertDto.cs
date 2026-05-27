namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarPagoProveedorUpsertDto
    {
        public int IdProveedor { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
    }
}