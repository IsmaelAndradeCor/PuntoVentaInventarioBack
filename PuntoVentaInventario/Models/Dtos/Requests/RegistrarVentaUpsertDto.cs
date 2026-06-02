using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarVentaUpsertDto
    {
        [Required]
        [MinLength(1)]
        public List<RegistrarVentaDetalleUpsertDto> Detalles { get; set; } = new();
        [Range(1, int.MaxValue, ErrorMessage = "El método de pago es obligatorio.")]
        public int IdMetodoPago { get; set; }
    }

    public class RegistrarVentaDetalleUpsertDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad de algunos productos debe ser mayor a 0 (cero)")]
        public decimal Cantidad { get; set; }
    }
}