using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class _1abril : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "IdUsuarioCreacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "IdUsuarioEliminacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "IdUsuarioModificacion",
                table: "Ventas");

            migrationBuilder.CreateTable(
                name: "ProductosDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecioCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Proveedor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductosDto");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Ventas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Ventas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Ventas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Ventas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuarioCreacion",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuarioEliminacion",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuarioModificacion",
                table: "Ventas",
                type: "int",
                nullable: true);
        }
    }
}
