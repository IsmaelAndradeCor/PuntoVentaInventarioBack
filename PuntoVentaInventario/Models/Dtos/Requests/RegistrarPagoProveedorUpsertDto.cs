namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarPagoProveedorUpsertDto
    {
        public int IdProveedor { get; set; }
        public decimal Monto { get; set; }
        public int IdMetodoPago { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
    }
}