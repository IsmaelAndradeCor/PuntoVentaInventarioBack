namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}