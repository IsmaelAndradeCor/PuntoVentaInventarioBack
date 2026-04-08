namespace PuntoVentaInventario.Models
{
    public class GenerarVentasDto
    {
        public String Folio { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Ganancias { get; set; }
        public String FormaPago { get; set; }

    }
}
