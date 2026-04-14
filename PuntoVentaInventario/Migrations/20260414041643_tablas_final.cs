using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class tablas_final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrecioVenta",
                table: "ProductosDto",
                newName: "Precio");

            migrationBuilder.RenameColumn(
                name: "PrecioCompra",
                table: "ProductosDto",
                newName: "Costo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "ProductosDto",
                newName: "PrecioVenta");

            migrationBuilder.RenameColumn(
                name: "Costo",
                table: "ProductosDto",
                newName: "PrecioCompra");
        }
    }
}
