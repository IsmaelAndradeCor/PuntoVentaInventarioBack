namespace PuntoVentaInventario.Models.Dtos.Responses
{
    public class UnidadMedidaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public bool PermiteDecimales { get; set; }
    }
}
