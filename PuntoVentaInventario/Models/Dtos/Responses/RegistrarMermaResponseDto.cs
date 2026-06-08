namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class RegistrarMermaResponseDto
    {
        public int IdMerma { get; set; }
        public string Folio { get; set; } = string.Empty;
        public decimal CostoTotal { get; set; }
    }
}
