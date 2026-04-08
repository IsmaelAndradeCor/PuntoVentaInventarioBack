USE [master]
GO
/****** Object:  Database [PuntoVentaInventario]    Script Date: 07/04/2026 11:23:36 p. m. ******/
CREATE DATABASE [PuntoVentaInventario]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PuntoVentaInventario', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PuntoVentaInventario.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PuntoVentaInventario_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PuntoVentaInventario_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PuntoVentaInventario] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PuntoVentaInventario].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PuntoVentaInventario] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET ARITHABORT OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PuntoVentaInventario] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PuntoVentaInventario] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PuntoVentaInventario] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PuntoVentaInventario] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET RECOVERY FULL 
GO
ALTER DATABASE [PuntoVentaInventario] SET  MULTI_USER 
GO
ALTER DATABASE [PuntoVentaInventario] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PuntoVentaInventario] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PuntoVentaInventario] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PuntoVentaInventario] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PuntoVentaInventario] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PuntoVentaInventario] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'PuntoVentaInventario', N'ON'
GO
ALTER DATABASE [PuntoVentaInventario] SET QUERY_STORE = ON
GO
ALTER DATABASE [PuntoVentaInventario] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PuntoVentaInventario]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleVentas]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleVentas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdVenta] [int] NOT NULL,
	[IdProducto] [int] NOT NULL,
	[CodigoProducto] [nvarchar](20) NOT NULL,
	[NombreProducto] [nvarchar](100) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
	[PrecioTotal] [decimal](18, 2) NOT NULL,
	[CostoTotal] [decimal](18, 2) NOT NULL,
	[CostoUnitario] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_DetalleVentas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Productos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](20) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Descripcion] [nvarchar](500) NULL,
	[PrecioCompra] [decimal](18, 2) NOT NULL,
	[PrecioVenta] [decimal](18, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[Categoria] [nvarchar](50) NULL,
	[Proveedor] [nvarchar](100) NULL,
	[FechaCreacion] [datetime2](7) NOT NULL,
	[FechaModificacion] [datetime2](7) NULL,
	[FechaEliminacion] [datetime2](7) NULL,
	[IdUsuarioCreacion] [int] NOT NULL,
	[IdUsuarioModificacion] [int] NULL,
	[IdUsuarioEliminacion] [int] NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductosDto]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductosDto](
	[Id] [int] NOT NULL,
	[Codigo] [nvarchar](max) NOT NULL,
	[Nombre] [nvarchar](max) NOT NULL,
	[Descripcion] [nvarchar](max) NOT NULL,
	[PrecioCompra] [decimal](18, 2) NOT NULL,
	[PrecioVenta] [decimal](18, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[Categoria] [nvarchar](max) NULL,
	[Proveedor] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductosV2]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductosV2](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](20) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Descripcion] [varchar](255) NULL,
	[PrecioCompra] [decimal](10, 2) NOT NULL,
	[PrecioVenta] [decimal](10, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[StockMinimo] [int] NULL,
	[Categoria] [varchar](50) NULL,
	[Proveedor] [varchar](100) NULL,
	[FechaCreacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ventas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Folio] [nvarchar](20) NOT NULL,
	[FechaVenta] [datetime2](7) NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[FormaPago] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Ventas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_DetalleVentas_IdProducto]    Script Date: 07/04/2026 11:23:36 p. m. ******/
CREATE NONCLUSTERED INDEX [IX_DetalleVentas_IdProducto] ON [dbo].[DetalleVentas]
(
	[IdProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DetalleVentas_IdVenta]    Script Date: 07/04/2026 11:23:36 p. m. ******/
CREATE NONCLUSTERED INDEX [IX_DetalleVentas_IdVenta] ON [dbo].[DetalleVentas]
(
	[IdVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Productos_Codigo]    Script Date: 07/04/2026 11:23:36 p. m. ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Productos_Codigo] ON [dbo].[Productos]
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Ventas_Folio]    Script Date: 07/04/2026 11:23:36 p. m. ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Ventas_Folio] ON [dbo].[Ventas]
(
	[Folio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProductosDto] ADD  DEFAULT (N'') FOR [Descripcion]
GO
ALTER TABLE [dbo].[DetalleVentas]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVentas_Productos_IdProducto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Productos] ([Id])
GO
ALTER TABLE [dbo].[DetalleVentas] CHECK CONSTRAINT [FK_DetalleVentas_Productos_IdProducto]
GO
ALTER TABLE [dbo].[DetalleVentas]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVentas_Ventas_IdVenta] FOREIGN KEY([IdVenta])
REFERENCES [dbo].[Ventas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DetalleVentas] CHECK CONSTRAINT [FK_DetalleVentas_Ventas_IdVenta]
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerarVentas]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GenerarVentas]

	@FechaInicio AS DATE = NULL,
	@FechaFin AS DATE = NULL,
	@Detalle AS BIT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Esto es para traer las ventas del día actual, sin detalle
	IF (@FechaInicio IS NULL AND @FechaFin IS NULL AND @Detalle = 0)
	BEGIN

		SELECT V.Folio, V.FechaVenta, SUM(D.CostoTotal) CostoTotal, V.Total, V.Total - SUM(D.CostoTotal) Ganancias,V.FormaPago
		FROM Ventas V
		INNER JOIN DetalleVentas D ON V.Id = D.IdVenta
		WHERE CAST(V.FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
		GROUP BY V.Folio, V.FechaVenta, V.Total, V.FormaPago

	END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProductosInsertar]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Insertar nuevo
CREATE   PROCEDURE [dbo].[sp_ProductosInsertar]
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

    IF EXISTS(SELECT 1 FROM Productos WHERE Codigo = @Codigo)
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

/****** Object:  StoredProcedure [dbo].[sp_ProductosListar]    Script Date: 01/04/2026 06:44:03 a. m. ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[sp_ProductosListar]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Listar todos productos
CREATE   PROCEDURE [dbo].[sp_ProductosListar]
AS
BEGIN
    SELECT Id, Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, Categoria, Proveedor 
    FROM Productos 
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProductosObtener]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Obtener uno por Id
CREATE   PROCEDURE [dbo].[sp_ProductosObtener]
    @Codigo varchar(20)
AS
BEGIN
    SELECT TOP 1 Id, Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, Categoria, Proveedor 
    FROM Productos 
    WHERE Codigo = @Codigo AND Activo = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProductosObtenerStockMinimo]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Gilberto Ismael Andrade Cortes
-- Create date: 31/03/2026
-- Description:	Obtiene los productos que llegaron al stock minimo y serán alertados
-- =============================================
CREATE   PROCEDURE [dbo].[sp_ProductosObtenerStockMinimo]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT Id, Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, Categoria, Proveedor 
	FROM Productos
	WHERE Stock <= StockMinimo AND Activo = 1

END
GO
/****** Object:  StoredProcedure [dbo].[sp_RegistrarVenta]    Script Date: 07/04/2026 11:23:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_RegistrarVenta]
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

        INSERT INTO DetalleVentas (IdVenta, IdProducto, CodigoProducto, NombreProducto, Cantidad, CostoUnitario, CostoTotal, PrecioUnitario, PrecioTotal)
        SELECT 
            @IdVenta,
            t.c.value('(IdProducto)[1]', 'INT'),
            t.c.value('(Codigo)[1]', 'NVARCHAR(50)'),
            t.c.value('(Nombre)[1]', 'NVARCHAR(100)'),
            t.c.value('(Cantidad)[1]', 'INT'),
            t.c.value('(Costo)[1]', 'DECIMAL(10,2)'),
            t.c.value('(Cantidad)[1]', 'INT') * t.c.value('(Costo)[1]', 'DECIMAL(10,2)'),
            t.c.value('(Precio)[1]', 'DECIMAL(10,2)'),
            t.c.value('(Cantidad)[1]', 'INT') * t.c.value('(Precio)[1]', 'DECIMAL(10,2)')
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
END;
GO
USE [master]
GO
ALTER DATABASE [PuntoVentaInventario] SET  READ_WRITE 
GO
