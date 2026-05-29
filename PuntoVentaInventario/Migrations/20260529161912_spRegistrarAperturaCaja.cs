using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class spRegistrarAperturaCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                
                CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarAperturaCaja]
                    @MontoInicial DECIMAL(18,2),
                    @IdUsuario NVARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    BEGIN TRY
                        BEGIN TRANSACTION;

                        IF @MontoInicial IS NULL OR @MontoInicial < 0
                            THROW 50201, 'El monto inicial debe ser mayor o igual a 0.', 1;

                        IF @IdUsuario IS NULL OR LTRIM(RTRIM(@IdUsuario)) = ''
                            THROW 50202, 'El usuario es obligatorio.', 1;

                        DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

                        IF EXISTS (
                            SELECT 1
                            FROM dbo.AperturasCaja WITH (UPDLOCK, HOLDLOCK)
                            WHERE FechaOperacion = @Hoy
                                AND Activo = 1
                        )
                            THROW 50203, 'La apertura de caja del día ya fue registrada.', 1;

                        INSERT INTO dbo.AperturasCaja
                        (
                            FechaOperacion,
                            MontoInicial,
                            FechaRegistro,
                            IdUsuario,
                            Activo
                        )
                        VALUES
                        (
                            @Hoy,
                            @MontoInicial,
                            GETDATE(),
                            @IdUsuario,
                            1
                        );

                        DECLARE @Id INT = SCOPE_IDENTITY();

                        COMMIT TRANSACTION;

                        SELECT
                            @Id AS Id,
                            @Hoy AS FechaOperacion,
                            @MontoInicial AS MontoInicial,
                            GETDATE() AS FechaRegistro,
                            @IdUsuario AS IdUsuario;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        THROW;
                    END CATCH
                END

            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
            DROP PROCEDURE IF EXISTS sp_RegistrarAperturaCaja;
            ");
        }
    }
}
