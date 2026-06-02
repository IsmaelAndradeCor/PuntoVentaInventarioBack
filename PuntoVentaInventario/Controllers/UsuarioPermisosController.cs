using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Administrador")]
    public class UsuarioPermisosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsuarioPermisosController> _logger;

        public UsuarioPermisosController(UserManager<ApplicationUser> userManager, ILogger<UsuarioPermisosController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        [Authorize(Policy = Permissions.PermisosUsuario.Ver)]
        [HttpGet("permisos/catalogo")]
        public IActionResult ObtenerCatalogoPermisos()
        {
            return Ok(Permissions.All.OrderBy(x => x).ToList());
        }

        [Authorize(Policy = Permissions.PermisosUsuario.Ver)]
        [HttpGet("catalogo-ui")]
        public IActionResult ObtenerCatalogoUi()
        {
            return Ok(PermissionsUiCatalog.Get());
        }

        [Authorize(Policy = Permissions.PermisosUsuario.Actualizar)]
        [HttpPost("{id}/permisos/{permission}")]
        public async Task<IActionResult> AsignarPermiso(string id, string permission)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(permission))
                    return BadRequest(new { mensaje = "Usuario y permiso son obligatorios." });

                if (!Permissions.All.Contains(permission))
                    return BadRequest(new { mensaje = "El permiso no existe en el catálogo." });

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var existingClaims = await _userManager.GetClaimsAsync(user);

                var alreadyHasPermission = existingClaims.Any(c => c.Type == "permission" && c.Value == permission);

                if (alreadyHasPermission)
                    return BadRequest(new { mensaje = "El usuario ya tiene ese permiso." });

                var result = await _userManager.AddClaimAsync(user, new Claim("permission", permission));

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo asignar el permiso.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Permiso asignado correctamente.",
                    userName = user.UserName,
                    permission = permission
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permiso");
                return StatusCode(500, new { mensaje = "Error interno al asignar permiso." });
            }
        }

        [Authorize(Policy = Permissions.PermisosUsuario.Actualizar)]
        [HttpDelete("{id}/permisos/{permission}")]
        public async Task<IActionResult> QuitarPermiso(string id, string permission)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(permission))
                    return BadRequest(new { mensaje = "Usuario y permiso son obligatorios." });

                if (!Permissions.All.Contains(permission))
                    return BadRequest(new { mensaje = "El permiso no existe en el catálogo." });

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var claims = await _userManager.GetClaimsAsync(user);

                var claimToRemove = claims.FirstOrDefault(c => c.Type == "permission" && c.Value == permission);

                if (claimToRemove == null)
                    return BadRequest(new { mensaje = "El usuario no tiene asignado ese permiso." });

                var result = await _userManager.RemoveClaimAsync(user, claimToRemove);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo quitar el permiso.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Permiso removido correctamente.",
                    userName = user.UserName,
                    permission = permission
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al quitar permiso");
                return StatusCode(500, new { mensaje = "Error interno al quitar permiso." });
            }
        }
    }
}