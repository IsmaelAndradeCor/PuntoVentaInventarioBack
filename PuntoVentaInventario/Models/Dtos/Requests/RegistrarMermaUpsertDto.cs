using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarMermaUpsertDto
    {
        [Required]
        [MinLength(1)]
        public List<RegistrarMermaDetalleUpsertDto> Detalles { get; set; } = new();

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }

    public class RegistrarMermaDetalleUpsertDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal Cantidad { get; set; }
    }
}
