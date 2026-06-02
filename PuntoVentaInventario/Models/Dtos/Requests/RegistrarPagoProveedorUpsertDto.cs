using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarPagoProveedorUpsertDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago es obligatorio.")]
        public int IdMetodoPago { get; set; }

        [StringLength(100)]
        public string? Referencia { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}