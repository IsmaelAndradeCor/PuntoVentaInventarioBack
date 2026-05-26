namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class GenerarVentasRequestDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool IncluirDetalle { get; set; } = false;
    }
}
