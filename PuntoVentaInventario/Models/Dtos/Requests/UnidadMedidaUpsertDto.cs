using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class UnidadMedidaUpsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 30 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "La clave debe tener entre 1 y 10 caracteres.")]
        public string Clave { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo de sí permite decimales es obligatorio.")]
        public bool PermiteDecimales { get; set; }
    }
}
