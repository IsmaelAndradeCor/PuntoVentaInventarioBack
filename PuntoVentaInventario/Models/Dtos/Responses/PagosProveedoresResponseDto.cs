namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class PagosProveedoresResponseDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string NombreProveedor { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public int IdMetodoPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
    }
}
