namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class CorteCajaHoyResponseDto
    {
        public DateTime FechaOperacion { get; set; }
        public decimal MontoInicialCaja { get; set; }
        public decimal MontoVentas { get; set; }
        public decimal MontoPagoProveedores { get; set; }
        public decimal CorteCaja { get; set; }

        public int? IdAperturaActiva { get; set; }
        public bool CortePendiente { get; set; }
        public string? IdUsuarioActivo { get; set; }
        public string? NombreUsuarioActivo { get; set; }

        public List<CorteRealizadoDto> CortesRealizados { get; set; } = new();
    }

    public class CorteRealizadoDto
    {
        public int Id { get; set; }
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
        public bool CorteFinal { get; set; }
        public string? Observaciones { get; set; }

        public List<GenerarVentaResponseDto> Ventas { get; set; } = new();
        public List<CorteDetallePagoDto> PagosProveedores { get; set; } = new();
    }

    //public class CorteDetalleVentaDto
    //{
    //    public int IdVenta { get; set; }
    //    public string Folio { get; set; } = string.Empty;
    //    public DateTime FechaVenta { get; set; }
    //    public decimal Total { get; set; }
    //    public List<GenerarVentaDetalleResponseDto> Detalles { get; set; } = new();
    //}

    public class CorteDetallePagoDto
    {
        public int IdPago { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Proveedor { get; set; } = string.Empty;
    }
}
