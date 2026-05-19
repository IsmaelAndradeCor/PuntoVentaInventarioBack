using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sp_GenerarVentas]
                @FechaInicio DATE = NULL,
                @FechaFin DATE = NULL,
                @Detalle BIT = 0
            AS
            BEGIN
                SET NOCOUNT ON;

                IF (@FechaInicio IS NULL AND @FechaFin IS NULL AND @Detalle = 0)
                BEGIN
                    SELECT 
                        V.Folio,
                        V.FechaVenta,
                        SUM(D.CostoTotal) AS CostoTotal,
                        V.Total,
                        V.Total - SUM(D.CostoTotal) AS Ganancias,
                        V.FormaPago
                    FROM Ventas V
                    INNER JOIN DetalleVentas D ON V.Id = D.IdVenta
                    WHERE CAST(V.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
                    GROUP BY V.Folio, V.FechaVenta, V.Total, V.FormaPago
                END
            END
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarVenta]
                @Folio NVARCHAR(20),
                @IdUsuario VARCHAR(450),
                @Total DECIMAL(10,2),
                @Detalle XML
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRY
                    BEGIN TRANSACTION;

                    DECLARE @Items TABLE
                    (
                        IdProducto INT,
                        Codigo NVARCHAR(50),
                        Nombre NVARCHAR(100),
                        Cantidad DECIMAL(18,2),
                        Costo DECIMAL(18,2),
                        Precio DECIMAL(18,2)
                    );

                    INSERT INTO @Items (IdProducto, Codigo, Nombre, Cantidad, Costo, Precio)
                    SELECT
                        t.c.value('(IdProducto)[1]', 'INT'),
                        t.c.value('(Codigo)[1]', 'NVARCHAR(50)'),
                        t.c.value('(Nombre)[1]', 'NVARCHAR(100)'),
                        t.c.value('(Cantidad)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Costo)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Precio)[1]', 'DECIMAL(18,2)')
                    FROM @Detalle.nodes('/Items/Item') t(c);

                    IF EXISTS (
                        SELECT 1
                        FROM @Items i
                        INNER JOIN Productos p ON p.Id = i.IdProducto
                        WHERE p.Stock < i.Cantidad
                    )
                    BEGIN
                        ;THROW 50001, 'Inventario insuficiente para uno o más productos.', 1;
                    END

                    INSERT INTO Ventas (Folio, FechaVenta, Subtotal, Descuento, Total, IdUsuario, FormaPago)
                    VALUES (@Folio, GETDATE(), @Total, 0, @Total, @IdUsuario, 'Efectivo');

                    DECLARE @IdVenta INT = SCOPE_IDENTITY();

                    INSERT INTO DetalleVentas
                    (
                        IdVenta,
                        IdProducto,
                        CodigoProducto,
                        NombreProducto,
                        Cantidad,
                        CostoUnitario,
                        CostoTotal,
                        PrecioUnitario,
                        PrecioTotal
                    )
                    SELECT
                        @IdVenta,
                        i.IdProducto,
                        i.Codigo,
                        i.Nombre,
                        i.Cantidad,
                        i.Costo,
                        i.Cantidad * i.Costo,
                        i.Precio,
                        i.Cantidad * i.Precio
                    FROM @Items i;

                    UPDATE p
                    SET p.Stock = p.Stock - i.Cantidad
                    FROM Productos p
                    INNER JOIN @Items i ON p.Id = i.IdProducto
                    WHERE p.Stock >= i.Cantidad;

                    IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @Items)
                    BEGIN
                        ;THROW 50002, 'No se pudo actualizar el inventario de todos los productos.', 1;
                    END

                    COMMIT TRANSACTION;

                    SELECT @IdVenta AS IdVenta;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION;

                    ;THROW;
                END CATCH
            END
            ", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_GenerarVentas];", suppressTransaction: true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_RegistrarVenta];", suppressTransaction: true);
        }
    }
}
