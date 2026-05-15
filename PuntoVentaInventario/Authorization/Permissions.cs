namespace PuntoVentaInventario.Authorization
{
    public static class Permissions
    {
        public static class Home
        {
            public const string Ver = "home.ver";
        }

        public static class Ventas
        {
            public const string Realizar = "ventas.realizar";
            public const string HistorialVer = "ventas.historial.ver";
        }

        public static class Productos
        {
            public const string Ver = "productos.ver";
            public const string Crear = "productos.crear";
            public const string Actualizar = "productos.actualizar";
            public const string Eliminar = "productos.eliminar";
        }

        public static class Marcas
        {
            public const string Ver = "marcas.ver";
            public const string Crear = "marcas.crear";
            public const string Actualizar = "marcas.actualizar";
            public const string Eliminar = "marcas.eliminar";
        }

        public static class Categorias
        {
            public const string Ver = "categorias.ver";
            public const string Crear = "categorias.crear";
            public const string Actualizar = "categorias.actualizar";
            public const string Eliminar = "categorias.eliminar";
        }

        public static class UnidadesMedida
        {
            public const string Ver = "unidadesmedida.ver";
            public const string Crear = "unidadesmedida.crear";
            public const string Actualizar = "unidadesmedida.actualizar";
            public const string Eliminar = "unidadesmedida.eliminar";
        }

        public static class Proveedores
        {
            public const string Ver = "proveedores.ver";
            public const string Crear = "proveedores.crear";
            public const string Actualizar = "proveedores.actualizar";
            public const string Eliminar = "proveedores.eliminar";
        }

        public static List<string> All => new()
        {
            Home.Ver,

            Ventas.Realizar,
            Ventas.HistorialVer,

            Productos.Ver,
            Productos.Crear,
            Productos.Actualizar,
            Productos.Eliminar,

            Marcas.Ver,
            Marcas.Crear,
            Marcas.Actualizar,
            Marcas.Eliminar,

            Categorias.Ver,
            Categorias.Crear,
            Categorias.Actualizar,
            Categorias.Eliminar,

            UnidadesMedida.Ver,
            UnidadesMedida.Crear,
            UnidadesMedida.Actualizar,
            UnidadesMedida.Eliminar,

            Proveedores.Ver,
            Proveedores.Crear,
            Proveedores.Actualizar,
            Proveedores.Eliminar
        };
    }
}