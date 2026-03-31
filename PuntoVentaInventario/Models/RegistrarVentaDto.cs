namespace PuntoVentaInventario.Models
{
    public class RegistrarVentaDto
    {
        public string Folio { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public decimal Total { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }
}