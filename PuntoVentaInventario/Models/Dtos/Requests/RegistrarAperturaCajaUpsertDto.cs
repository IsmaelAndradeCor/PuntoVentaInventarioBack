using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarAperturaCajaUpsertDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "El monto inicial debe ser mayor o igual a 0.")]
        public decimal MontoInicial { get; set; }
    }
}