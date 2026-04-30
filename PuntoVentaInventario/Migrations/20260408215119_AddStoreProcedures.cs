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
            CREATE OR ALTER PROCEDURE [dbo].[sp_ProductosInsertar]
                @Codigo VARCHAR(20),
                @Nombre VARCHAR(100),
                @Descripcion VARCHAR(255) = NULL,
                @PrecioCompra DECIMAL(10,2),
                @PrecioVenta DECIMAL(10,2),
                @Stock INT = 0,
                @StockMinimo INT = 0,
                @Categoria VARCHAR(50) = NULL,
                @Proveedor VARCHAR(100) = NULL,
                @IdUsuarioCreacion INT = 1
            AS
            BEGIN
                SET NOCOUNT ON;

                IF EXISTS (SELECT 1 FROM Productos WHERE Codigo = @Codigo)
                BEGIN
                    SELECT 0 AS Error;
                    RETURN;
                END

                INSERT INTO Productos
                (
                    Codigo,
                    Nombre,
                    Descripcion,
                    PrecioCompra,
                    PrecioVenta,
                    Stock,
                    StockMinimo,
                    Categoria,
                    Proveedor,
                    FechaCreacion,
                    IdUsuarioCreacion,
                    Activo
                )
                VALUES
                (
                    @Codigo,
                    @Nombre,
                    @Descripcion,
                    @PrecioCompra,
                    @PrecioVenta,
                    @Stock,
                    @StockMinimo,
                    @Categoria,
                    @Proveedor,
                    GETUTCDATE(),
                    @IdUsuarioCreacion,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT) AS NuevoId;
            END
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sp_ProductosListar]
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT 
                    Id,
                    Codigo,
                    Nombre,
                    Descripcion,
                    PrecioCompra,
                    PrecioVenta,
                    Stock,
                    StockMinimo,
                    Categoria,
                    Proveedor
                FROM Productos
                WHERE Activo = 1
                ORDER BY Nombre;
            END
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sp_ProductosObtener]
                @Codigo VARCHAR(20)
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT TOP 1
                    Id,
                    Codigo,
                    Nombre,
                    Descripcion,
                    PrecioCompra,
                    PrecioVenta,
                    Stock,
                    StockMinimo,
                    Categoria,
                    Proveedor
                FROM Productos
                WHERE Codigo = @Codigo
                  AND Activo = 1;
            END
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarVenta]
                @Folio NVARCHAR(20),
                @IdUsuario INT,
                @Total DECIMAL(10,2),
                @Detalle XML
            AS
            BEGIN
                SET NOCOUNT ON;

                BEGIN TRY
                    BEGIN TRANSACTION;

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
                        t.c.value('(IdProducto)[1]', 'INT'),
                        t.c.value('(Codigo)[1]', 'NVARCHAR(50)'),
                        t.c.value('(Nombre)[1]', 'NVARCHAR(100)'),
                        t.c.value('(Cantidad)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Costo)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Cantidad)[1]', 'DECIMAL(18,2)') * t.c.value('(Costo)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Precio)[1]', 'DECIMAL(18,2)'),
                        t.c.value('(Cantidad)[1]', 'DECIMAL(18,2)') * t.c.value('(Precio)[1]', 'DECIMAL(18,2)')
                    FROM @Detalle.nodes('/Items/Item') t(c);

                    UPDATE p
                    SET p.Stock = p.Stock - d.Cantidad
                    FROM Productos p
                    INNER JOIN DetalleVentas d ON p.Id = d.IdProducto
                    WHERE d.IdVenta = @IdVenta;

                    COMMIT TRANSACTION;

                    SELECT @IdVenta AS IdVenta;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    THROW;
                END CATCH
            END
            ", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_GenerarVentas];", suppressTransaction: true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_ProductosInsertar];", suppressTransaction: true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_ProductosListar];", suppressTransaction: true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_ProductosObtener];", suppressTransaction: true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_RegistrarVenta];", suppressTransaction: true);
        }
    }
}
