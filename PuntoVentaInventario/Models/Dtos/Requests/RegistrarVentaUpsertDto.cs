namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarVentaUpsertDto
    {
        public string Folio { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }
}