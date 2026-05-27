namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class PagosProveedoresResponseDto
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public string NombreProveedor { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
