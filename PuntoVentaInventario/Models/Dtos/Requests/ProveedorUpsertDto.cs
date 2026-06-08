using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class ProveedorUpsertDto
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres para el nombre del proveedor.")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres para el contacto del proveedor.")]
        public string? Contacto { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres para el teléfono del proveedor.")]
        [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "El teléfono solo puede contener números, espacios, paréntesis, + y guiones.")]
        public string? Telefono { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres para el correo del proveedor.")]
        public string? Correo { get; set; }
    }
}