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
    [Authorize(Roles = "Administrador")]
    public class UsuariosPermisosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosPermisosController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("catalogo")]
        public IActionResult ObtenerCatalogoPermisos()
        {
            return Ok(Permissions.All.OrderBy(x => x).ToList());
        }

        [HttpGet("catalogo-usuarios")]
        public async Task<ActionResult<List<UsuarioPermisosResponseDto>>> ObtenerCatalogoUsuarios()
        {
            try
            {
                var usuarios = await _userManager.Users
                    .OrderBy(u => u.UserName)
                    .ToListAsync();

                var response = new List<UsuarioPermisosResponseDto>();

                foreach (var user in usuarios)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = await _userManager.GetClaimsAsync(user);

                    var permissions = claims
                        .Where(c => c.Type == "permission")
                        .Select(c => c.Value)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList();

                    response.Add(new UsuarioPermisosResponseDto
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        NombreCompleto = user.NombreCompleto,
                        Activo = user.Activo,
                        Roles = roles,
                        Permissions = permissions
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = $"Error al obtener el catálogo de usuarios: {ex.Message}"
                });
            }
        }

        [HttpGet("usuario/{userName}")]
        public async Task<ActionResult<UsuarioPermisosResponseDto>> ObtenerUsuarioPorUserName(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName.Trim());

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                var permissions = claims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                var response = new UsuarioPermisosResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    NombreCompleto = user.NombreCompleto,
                    Activo = user.Activo,
                    Roles = roles,
                    Permissions = permissions
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = $"Error al obtener el usuario: {ex.Message}"
                });
            }
        }

        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarPermiso([FromBody] AssignPermissionUpsertDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Permission))
                    return BadRequest(new { mensaje = "Usuario y permiso son obligatorios." });

                if (!Permissions.All.Contains(dto.Permission))
                    return BadRequest(new { mensaje = "El permiso no existe en el catálogo." });

                var user = await _userManager.FindByNameAsync(dto.UserName.Trim());
                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var existingClaims = await _userManager.GetClaimsAsync(user);
                var alreadyHasPermission = existingClaims.Any(c => c.Type == "permission" && c.Value == dto.Permission);

                if (alreadyHasPermission)
                    return BadRequest(new { mensaje = "El usuario ya tiene ese permiso." });

                var result = await _userManager.AddClaimAsync(user, new Claim("permission", dto.Permission));

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
                    permission = dto.Permission
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al asignar permiso: {ex.Message}" });
            }
        }

        [HttpPost("quitar")]
        public async Task<IActionResult> QuitarPermiso([FromBody] RemovePermissionUpsertDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Permission))
                    return BadRequest(new { mensaje = "Usuario y permiso son obligatorios." });

                var user = await _userManager.FindByNameAsync(dto.UserName.Trim());
                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var claims = await _userManager.GetClaimsAsync(user);
                var claimToRemove = claims.FirstOrDefault(c => c.Type == "permission" && c.Value == dto.Permission);

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
                    permission = dto.Permission
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al quitar permiso: {ex.Message}" });
            }
        }
    }
}