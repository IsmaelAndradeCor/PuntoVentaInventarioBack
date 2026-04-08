using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class _3abril : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "DetalleVentas",
                newName: "PrecioTotal");

            migrationBuilder.AddColumn<decimal>(
                name: "CostoTotal",
                table: "DetalleVentas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoUnitario",
                table: "DetalleVentas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoTotal",
                table: "DetalleVentas");

            migrationBuilder.DropColumn(
                name: "CostoUnitario",
                table: "DetalleVentas");

            migrationBuilder.RenameColumn(
                name: "PrecioTotal",
                table: "DetalleVentas",
                newName: "Subtotal");
        }
    }
}
