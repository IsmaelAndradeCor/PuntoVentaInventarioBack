namespace PuntoVentaInventario.Authorization
{
    public static class Permissions
    {
        public static class Home
        {
            public const string Ver = "home.ver";
        }

        public static class Configuracion
        {
            public const string Ver = "configuracion.ver";
        }

        public static class Usuarios
        {
            public const string Ver = "usuarios.ver";

            public const string ActivosVer = "usuarios.activos.ver";
            public const string InactivosVer = "usuarios.inactivos.ver";

            public const string Crear = "usuarios.crear";
            public const string Actualizar = "usuarios.actualizar";

            public const string Activar = "usuarios.activar";
            public const string Desactivar = "usuarios.desactivar";
        }

        public static class PermisosUsuario
        {
            public const string Ver = "usuarios.permisos.ver";

            public const string Actualizar = "usuarios.permisos.actualizar";
        }

        public static class Ventas
        {
            public const string Ver = "ventas.ver";

            public const string Realizar = "ventas.realizar";
            public const string HistorialVer = "ventas.historial.ver";
        }

        public static class Mermas
        {
            public const string Ver = "mermas.ver";
            public const string Registrar = "mermas.registrar";
            public const string HistorialVer = "mermas.historial.ver";
        }

        public static class Productos
        {
            public const string Ver = "productos.ver";

            public const string ActivosVer = "productos.activos.ver";
            public const string InactivosVer = "productos.inactivos.ver";

            public const string Crear = "productos.crear";
            public const string Actualizar = "productos.actualizar";

            public const string Activar = "productos.activar";
            public const string Desactivar = "productos.desactivar";
        }

        public static class Marcas
        {
            public const string Ver = "marcas.ver";

            public const string ActivosVer = "marcas.activos.ver";
            public const string InactivosVer = "marcas.inactivos.ver";

            public const string Crear = "marcas.crear";
            public const string Actualizar = "marcas.actualizar";

            public const string Activar = "marcas.activar";
            public const string Desactivar = "marcas.desactivar";
        }

        public static class Categorias
        {
            public const string Ver = "categorias.ver";

            public const string ActivosVer = "categorias.activos.ver";
            public const string InactivosVer = "categorias.inactivos.ver";

            public const string Crear = "categorias.crear";
            public const string Actualizar = "categorias.actualizar";

            public const string Activar = "categorias.activar";
            public const string Desactivar = "categorias.desactivar";
        }

        public static class UnidadesMedida
        {
            public const string Ver = "unidadesmedida.ver";

            public const string ActivosVer = "unidadesmedida.activos.ver";
            public const string InactivosVer = "unidadesmedida.inactivos.ver";

            public const string Crear = "unidadesmedida.crear";
            public const string Actualizar = "unidadesmedida.actualizar";

            public const string Activar = "unidadesmedida.activar";
            public const string Desactivar = "unidadesmedida.desactivar";
        }

        public static class Proveedores
        {
            public const string Ver = "proveedores.ver";

            public const string ActivosVer = "proveedores.activos.ver";
            public const string InactivosVer = "proveedores.inactivos.ver";

            public const string Crear = "proveedores.crear";
            public const string Actualizar = "proveedores.actualizar";

            public const string Activar = "proveedores.activar";
            public const string Desactivar = "proveedores.desactivar";

            public const string Pagar = "proveedores.pagar";
            public const string PagosVer = "proveedores.pagos.ver";
        }

        public static List<string> All => new()
        {
            Home.Ver,

            Configuracion.Ver,

            Usuarios.Ver,
            Usuarios.ActivosVer,
            Usuarios.InactivosVer,
            Usuarios.Crear,
            Usuarios.Actualizar,
            Usuarios.Activar,
            Usuarios.Desactivar,

            PermisosUsuario.Ver,
            PermisosUsuario.Actualizar,

            Ventas.Ver,
            Ventas.Realizar,
            Ventas.HistorialVer,

            Mermas.Ver,
            Mermas.Registrar,
            Mermas.HistorialVer,

            Productos.Ver,
            Productos.ActivosVer,
            Productos.InactivosVer,
            Productos.Crear,
            Productos.Actualizar,
            Productos.Activar,
            Productos.Desactivar,

            Marcas.Ver,
            Marcas.ActivosVer,
            Marcas.InactivosVer,
            Marcas.Crear,
            Marcas.Actualizar,
            Marcas.Activar,
            Marcas.Desactivar,

            Categorias.Ver,
            Categorias.ActivosVer,
            Categorias.InactivosVer,
            Categorias.Crear,
            Categorias.Actualizar,
            Categorias.Activar,
            Categorias.Desactivar,

            UnidadesMedida.Ver,
            UnidadesMedida.ActivosVer,
            UnidadesMedida.InactivosVer,
            UnidadesMedida.Crear,
            UnidadesMedida.Actualizar,
            UnidadesMedida.Activar,
            UnidadesMedida.Desactivar,

            Proveedores.Ver,
            Proveedores.ActivosVer,
            Proveedores.InactivosVer,
            Proveedores.Crear,
            Proveedores.Actualizar,
            Proveedores.Activar,
            Proveedores.Desactivar,
            Proveedores.Pagar,
            Proveedores.PagosVer
        };
    }
}