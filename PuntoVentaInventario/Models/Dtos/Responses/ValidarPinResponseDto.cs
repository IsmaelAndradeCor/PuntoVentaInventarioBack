namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class ValidarPinResponseDto
    {
        public bool Autorizado { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
