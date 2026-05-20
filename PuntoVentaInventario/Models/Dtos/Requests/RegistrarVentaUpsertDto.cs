using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RegistrarVentaUpsertDto
    {
        [Required]
        [MinLength(1)]
        public List<RegistrarVentaDetalleUpsertDto> Detalles { get; set; } = new();
    }

    public class RegistrarVentaDetalleUpsertDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999")]
        public decimal Cantidad { get; set; }
    }
}