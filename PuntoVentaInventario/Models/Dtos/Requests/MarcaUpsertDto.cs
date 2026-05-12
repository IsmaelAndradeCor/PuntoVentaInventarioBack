using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class MarcaUpsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 20 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}