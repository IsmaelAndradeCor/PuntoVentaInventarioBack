namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class MermaResponseDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime FechaMerma { get; set; }
        public decimal CostoTotal { get; set; }
        public string? Observaciones { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
        public List<MermaDetalleResponseDto> Detalles { get; set; } = new();
    }

    public class MermaDetalleResponseDto
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public string CodigoProducto { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoTotal { get; set; }
    }
}
