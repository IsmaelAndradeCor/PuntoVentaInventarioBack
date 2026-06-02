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

        IF EXISTS (
            SELECT 1
            FROM dbo.AperturasCaja WITH (UPDLOCK, HOLDLOCK)
            WHERE Activo = 1
        )
            THROW 50203, 'Ya existe un turno de caja activo. Debe realizar el corte antes de abrir uno nuevo.', 1;

        DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

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
