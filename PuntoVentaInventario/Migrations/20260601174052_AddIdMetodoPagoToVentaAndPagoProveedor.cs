using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class AddIdMetodoPagoToVentaAndPagoProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PASO 1 - Agregar columnas nullable para no romper filas existentes
            migrationBuilder.AddColumn<int>(
                name: "IdMetodoPago",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdMetodoPago",
                table: "PagosProveedores",
                type: "int",
                nullable: true);

            // PASO 2 - Poblar IdMetodoPago desde el texto que ya existía
            migrationBuilder.Sql(@"
                UPDATE v
                SET v.IdMetodoPago = m.Id
                FROM Ventas v
                INNER JOIN MetodosPago m ON m.Nombre = v.FormaPago
                WHERE v.IdMetodoPago IS NULL;

                -- Fallback: si no hubo match exacto, asigna Efectivo (Id=1)
                UPDATE Ventas
                SET IdMetodoPago = 1
                WHERE IdMetodoPago IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.IdMetodoPago = m.Id
                FROM PagosProveedores p
                INNER JOIN MetodosPago m ON m.Nombre = p.MetodoPago
                WHERE p.IdMetodoPago IS NULL;

                -- Fallback: si no hubo match exacto, asigna Efectivo (Id=1)
                UPDATE PagosProveedores
                SET IdMetodoPago = 1
                WHERE IdMetodoPago IS NULL;
            ");

            // PASO 3 - Hacer NOT NULL con SQL directo (evita que EF genere DROP INDEX antes de que exista)
            migrationBuilder.Sql(@"
                ALTER TABLE Ventas
                ALTER COLUMN IdMetodoPago int NOT NULL;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE PagosProveedores
                ALTER COLUMN IdMetodoPago int NOT NULL;
            ");

            // PASO 4 - Crear índices (EF los necesita para las FK)
            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdMetodoPago",
                table: "Ventas",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_IdMetodoPago",
                table: "PagosProveedores",
                column: "IdMetodoPago");

            // PASO 5 - Crear foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_IdMetodoPago",
                table: "Ventas",
                column: "IdMetodoPago",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PagosProveedores_MetodosPago_IdMetodoPago",
                table: "PagosProveedores",
                column: "IdMetodoPago",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // PASO 6 - Eliminar las columnas de texto que ya no se necesitan
            migrationBuilder.DropColumn(
                name: "FormaPago",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "PagosProveedores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // PASO 1 - Quitar FK e índices
            migrationBuilder.DropForeignKey(
                name: "FK_PagosProveedores_MetodosPago_IdMetodoPago",
                table: "PagosProveedores");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_IdMetodoPago",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_IdMetodoPago",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_PagosProveedores_IdMetodoPago",
                table: "PagosProveedores");

            // PASO 2 - Restaurar columnas de texto como nullable primero
            migrationBuilder.AddColumn<string>(
                name: "FormaPago",
                table: "Ventas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "PagosProveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // PASO 3 - Recuperar el nombre del método desde la FK
            migrationBuilder.Sql(@"
                UPDATE v
                SET v.FormaPago = m.Nombre
                FROM Ventas v
                INNER JOIN MetodosPago m ON m.Id = v.IdMetodoPago;

                UPDATE Ventas
                SET FormaPago = 'Efectivo'
                WHERE FormaPago IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.MetodoPago = m.Nombre
                FROM PagosProveedores p
                INNER JOIN MetodosPago m ON m.Id = p.IdMetodoPago;

                UPDATE PagosProveedores
                SET MetodoPago = 'Efectivo'
                WHERE MetodoPago IS NULL;
            ");

            // PASO 4 - Hacer NOT NULL con SQL directo
            migrationBuilder.Sql(@"
                ALTER TABLE Ventas
                ALTER COLUMN FormaPago nvarchar(20) NOT NULL;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE PagosProveedores
                ALTER COLUMN MetodoPago nvarchar(50) NOT NULL;
            ");

            // PASO 5 - Eliminar las columnas FK que ya no se necesitan
            migrationBuilder.DropColumn(
                name: "IdMetodoPago",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "IdMetodoPago",
                table: "PagosProveedores");
        }
    }
}