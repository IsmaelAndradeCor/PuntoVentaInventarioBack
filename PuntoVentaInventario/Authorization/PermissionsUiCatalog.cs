using PuntoVentaInventario.Models.Dtos.Responses;

namespace PuntoVentaInventario.Authorization
{
    public static class PermissionsUiCatalog
    {
        public static List<PermisoNodoDto> Get()
        {
            return new List<PermisoNodoDto>
            {
                new()
                {
                    Key = "home",
                    Titulo = "Inicio",
                    Permission = Permissions.Home.Ver,
                    Hijos =
                    [
                        new()
                        {
                            Key = Permissions.Home.StockMinimoVer,
                            Titulo = "Stock Minimo",
                            Permission = Permissions.Home.StockMinimoVer                        
                        }
                    ]
                },
                new()
                {
                    Key = "configuracion",
                    Titulo = "Configuración",
                    Permission = Permissions.Configuracion.Ver,
                    Hijos =
                    [
                        new()
                        {
                            Key = "usuarios",
                            Titulo = "Usuarios",
                            Permission = Permissions.Usuarios.Ver,
                            Hijos =
                            [
                                new() { Key = "usuarios.activos.ver", Titulo = "Ver activos", Permission = Permissions.Usuarios.ActivosVer },
                                new() { Key = "usuarios.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.Usuarios.InactivosVer },
                                new() { Key = "usuarios.crear", Titulo = "Crear", Permission = Permissions.Usuarios.Crear },
                                new() { Key = "usuarios.actualizar", Titulo = "Actualizar", Permission = Permissions.Usuarios.Actualizar },
                                new() { Key = "usuarios.activar", Titulo = "Activar", Permission = Permissions.Usuarios.Activar },
                                new() { Key = "usuarios.desactivar", Titulo = "Desactivar", Permission = Permissions.Usuarios.Desactivar }
                            ]
                        },
                        new()
                        {
                            Key = "usuarios.permisos",
                            Titulo = "Permisos de usuario",
                            Permission = Permissions.PermisosUsuario.Ver,
                            Hijos =
                            [
                                new() { Key = "usuarios.permisos.actualizar", Titulo = "Actualizar", Permission = Permissions.PermisosUsuario.Actualizar }
                            ]
                        }
                    ]
                },
                new()
                {
                    Key = "ventas",
                    Titulo = "Ventas",
                    Permission = Permissions.Ventas.Ver,
                    Hijos =
                    [
                        new() { Key = "ventas.realizar", Titulo = "Realizar", Permission = Permissions.Ventas.Realizar },
                        new() { Key = "ventas.historial.ver", Titulo = "Ver historial", Permission = Permissions.Ventas.HistorialVer }
                    ]
                },
                new()
                {
                    Key = "mermas",
                    Titulo = "Mermas",
                    Permission = Permissions.Mermas.Ver,
                    Hijos =
                    [
                        new() { Key = "mermas.registrar", Titulo = "Registrar", Permission = Permissions.Mermas.Registrar },
                        new() { Key = "mermas.historial.ver", Titulo = "Ver historial", Permission = Permissions.Mermas.HistorialVer }
                    ]
                },
                new()
                {
                    Key = "cortecaja",
                    Titulo = "Corte Caja",
                    Permission = Permissions.CorteCaja.Ver,
                    Hijos =
                    [
                        new() { Key = Permissions.CorteCaja.Realizar, Titulo = "Realizar corte", Permission = Permissions.CorteCaja.Realizar }
                    ]
                },
                new()
                {
                    Key = "productos",
                    Titulo = "Productos",
                    Permission = Permissions.Productos.Ver,
                    Hijos =
                    [
                        new() { Key = "productos.activos.ver", Titulo = "Ver activos", Permission = Permissions.Productos.ActivosVer },
                        new() { Key = "productos.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.Productos.InactivosVer },
                        new() { Key = "productos.crear", Titulo = "Crear", Permission = Permissions.Productos.Crear },
                        new() { Key = "productos.actualizar", Titulo = "Actualizar", Permission = Permissions.Productos.Actualizar },
                        new() { Key = "productos.activar", Titulo = "Activar", Permission = Permissions.Productos.Activar },
                        new() { Key = "productos.desactivar", Titulo = "Desactivar", Permission = Permissions.Productos.Desactivar }
                    ]
                },
                new()
                {
                    Key = "marcas",
                    Titulo = "Marcas",
                    Permission = Permissions.Marcas.Ver,
                    Hijos =
                    [
                        new() { Key = "marcas.activos.ver", Titulo = "Ver activos", Permission = Permissions.Marcas.ActivosVer },
                        new() { Key = "marcas.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.Marcas.InactivosVer },
                        new() { Key = "marcas.crear", Titulo = "Crear", Permission = Permissions.Marcas.Crear },
                        new() { Key = "marcas.actualizar", Titulo = "Actualizar", Permission = Permissions.Marcas.Actualizar },
                        new() { Key = "marcas.activar", Titulo = "Activar", Permission = Permissions.Marcas.Activar },
                        new() { Key = "marcas.desactivar", Titulo = "Desactivar", Permission = Permissions.Marcas.Desactivar }
                    ]
                },
                new()
                {
                    Key = "categorias",
                    Titulo = "Categorías",
                    Permission = Permissions.Categorias.Ver,
                    Hijos =
                    [
                        new() { Key = "categorias.activos.ver", Titulo = "Ver activos", Permission = Permissions.Categorias.ActivosVer },
                        new() { Key = "categorias.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.Categorias.InactivosVer },
                        new() { Key = "categorias.crear", Titulo = "Crear", Permission = Permissions.Categorias.Crear },
                        new() { Key = "categorias.actualizar", Titulo = "Actualizar", Permission = Permissions.Categorias.Actualizar },
                        new() { Key = "categorias.activar", Titulo = "Activar", Permission = Permissions.Categorias.Activar },
                        new() { Key = "categorias.desactivar", Titulo = "Desactivar", Permission = Permissions.Categorias.Desactivar }
                    ]
                },
                new()
                {
                    Key = "unidadesmedida",
                    Titulo = "Unidades de medida",
                    Permission = Permissions.UnidadesMedida.Ver,
                    Hijos =
                    [
                        new() { Key = "unidadesmedida.activos.ver", Titulo = "Ver activos", Permission = Permissions.UnidadesMedida.ActivosVer },
                        new() { Key = "unidadesmedida.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.UnidadesMedida.InactivosVer },
                        new() { Key = "unidadesmedida.crear", Titulo = "Crear", Permission = Permissions.UnidadesMedida.Crear },
                        new() { Key = "unidadesmedida.actualizar", Titulo = "Actualizar", Permission = Permissions.UnidadesMedida.Actualizar },
                        new() { Key = "unidadesmedida.activar", Titulo = "Activar", Permission = Permissions.UnidadesMedida.Activar },
                        new() { Key = "unidadesmedida.desactivar", Titulo = "Desactivar", Permission = Permissions.UnidadesMedida.Desactivar }
                    ]
                },
                new()
                {
                    Key = "proveedores",
                    Titulo = "Proveedores",
                    Permission = Permissions.Proveedores.Ver,
                    Hijos =
                    [
                        new() { Key = "proveedores.activos.ver", Titulo = "Ver activos", Permission = Permissions.Proveedores.ActivosVer },
                        new() { Key = "proveedores.inactivos.ver", Titulo = "Ver inactivos", Permission = Permissions.Proveedores.InactivosVer },
                        new() { Key = "proveedores.crear", Titulo = "Crear", Permission = Permissions.Proveedores.Crear },
                        new() { Key = "proveedores.actualizar", Titulo = "Actualizar", Permission = Permissions.Proveedores.Actualizar },
                        new() { Key = "proveedores.activar", Titulo = "Activar", Permission = Permissions.Proveedores.Activar },
                        new() { Key = "proveedores.desactivar", Titulo = "Desactivar", Permission = Permissions.Proveedores.Desactivar },
                        new() { Key = "proveedores.pagar", Titulo = "Pagar a proveedores", Permission = Permissions.Proveedores.Pagar },
                        new() { Key = Permissions.Proveedores.PagosVer, Titulo = "Pagos a proveedores", Permission = Permissions.Proveedores.PagosVer }
                    ]
                }
            };
        }
    }
}