namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class CorteCajaResponseDto
    {
        public int Id { get; set; }
        public int IdAperturaCaja { get; set; }
        public DateTime FechaCorte { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal MontoVentasEfectivo { get; set; }
        public decimal MontoPagoProveedores { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal Retiro { get; set; }
        public decimal MontoFinal { get; set; }
        public string IdUsuarioPrevio { get; set; } = string.Empty;
        public string NombreUsuarioPrevio { get; set; } = string.Empty;
        public string IdUsuarioCorte { get; set; } = string.Empty;
        public string NombreUsuarioCorte { get; set; } = string.Empty;
        public string? IdUsuarioRecepcion { get; set; }
        public string? NombreUsuarioRecepcion { get; set; }
        public string? Observaciones { get; set; }

        public int? NuevoIdApertura { get; set; }
        public decimal? NuevoMontoInicial { get; set; }
        public bool CorteFinal { get; set; }
    }
}
