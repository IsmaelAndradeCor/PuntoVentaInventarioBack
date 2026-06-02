using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;
using System.Security.Claims;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            ILogger<UsuariosController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = Permissions.Usuarios.ActivosVer)]
        [HttpGet("activos")]
        public async Task<ActionResult<List<UsuarioPermisosResponseDto>>> ObtenerUsuariosActivos()
        {
            try
            {
                return Ok(await ObtenerUsuariosAsync(u => u.Activo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios activos");
                return StatusCode(500, new { mensaje = "Error interno al obtener el catálogo de usuarios." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.InactivosVer)]
        [HttpGet("inactivos")]
        public async Task<ActionResult<List<UsuarioPermisosResponseDto>>> ObtenerUsuariosInactivos()
        {
            try
            {
                return Ok(await ObtenerUsuariosAsync(u => !u.Activo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios inactivos");
                return StatusCode(500, new { mensaje = "Error interno al obtener el catálogo de usuarios." });
            }
        }

        private async Task<List<UsuarioPermisosResponseDto>> ObtenerUsuariosAsync(System.Linq.Expressions.Expression<Func<ApplicationUser, bool>> filtro)
        {
            var usuarios = await _userManager.Users
                .Where(filtro)
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userIds = usuarios.Select(u => u.Id).ToHashSet();

            var rolesDict = await _context.Roles
                .Where(r => _context.UserRoles.Any(ur => ur.RoleId == r.Id && userIds.Contains(ur.UserId)))
                .SelectMany(r => _context.UserRoles.Where(ur => ur.RoleId == r.Id && userIds.Contains(ur.UserId))
                    .Select(ur => new { ur.UserId, RoleName = r.Name! }))
                .ToListAsync();

            var rolesPorUsuario = rolesDict
                .GroupBy(r => r.UserId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.RoleName).ToList());

            var claims = await _context.UserClaims
                .Where(c => c.ClaimType == "permission" && userIds.Contains(c.UserId))
                .Select(c => new { c.UserId, c.ClaimValue })
                .ToListAsync();

            var claimsPorUsuario = claims
                .GroupBy(c => c.UserId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.ClaimValue!).Distinct().OrderBy(x => x).ToList());

            return usuarios.Select(user => new UsuarioPermisosResponseDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                NombreCompleto = user.NombreCompleto,
                Activo = user.Activo,
                Roles = rolesPorUsuario.GetValueOrDefault(user.Id, new List<string>()),
                Permissions = claimsPorUsuario.GetValueOrDefault(user.Id, new List<string>())
            }).ToList();
        }

        [Authorize(Policy = Permissions.Usuarios.Actualizar)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioPermisosResponseDto>> ObtenerUsuario(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

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
                _logger.LogError(ex, "Error al obtener el usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al obtener el usuario." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Crear)]
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioUpsertDto dto)
        {
            try
            {
                if (dto.Rol != "Empleado")
                {
                    return BadRequest(new { mensaje = "Solo puede crear usuarios con el Rol de Empleado." });
                }

                var usuarioExistente = await _userManager.FindByNameAsync(dto.UserName.Trim());
                if (usuarioExistente != null)
                {
                    return BadRequest(new { mensaje = "El nombre de usuario ya existe." });
                }

                if (!await _roleManager.RoleExistsAsync(dto.Rol))
                {
                    return BadRequest(new { mensaje = $"El rol '{dto.Rol}' no existe." });
                }

                var nuevoUsuario = new ApplicationUser
                {
                    UserName = dto.UserName.Trim(),
                    NombreCompleto = dto.NombreCompleto.Trim(),
                    Activo = true
                };

                var result = await _userManager.CreateAsync(nuevoUsuario, dto.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        //mensaje = "No se pudo crear el usuario.",
                        mensaje = result.Errors.Select(e => e.Description)
                    });
                }

                var roleResult = await _userManager.AddToRoleAsync(nuevoUsuario, dto.Rol);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(nuevoUsuario);

                    return BadRequest(new
                    {
                        mensaje = "No se pudo asignar el rol al usuario.",
                        errores = roleResult.Errors.Select(e => e.Description)
                    });
                }

                return Ok();

                //return Ok(new
                //{
                //    mensaje = "Usuario creado correctamente.",  
                //    userName = nuevoUsuario.UserName,
                //    nombreCompleto = nuevoUsuario.NombreCompleto,
                //    rol = dto.Rol
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                return StatusCode(500, new { mensaje = "Error interno al crear el usuario." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Actualizar)]
        [HttpPut("{id}/nombre-completo")]
        public async Task<IActionResult> CambiarNombreCompleto(string id, [FromBody] CambiarNombreCompletoUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                    return BadRequest(new { mensaje = "El nombre completo es obligatorio." });

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                user.NombreCompleto = dto.NombreCompleto.Trim();

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo actualizar el nombre completo.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Nombre completo actualizado correctamente.",
                    userName = user.UserName,
                    nombreCompleto = user.NombreCompleto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el nombre completo del usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al actualizar el nombre completo." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Actualizar)]
        [HttpPut("{id}/rol")]
        public async Task<IActionResult> CambiarRol(string id, [FromBody] CambiarRolUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Rol))
                    return BadRequest(new { mensaje = "El rol es obligatorio." });

                if (dto.Rol != "Empleado")
                    return BadRequest(new { mensaje = "Solo se permite asignar el rol de Empleado." });

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                if (user.UserName == "admin")
                    return BadRequest(new { mensaje = "No se puede modificar el rol del usuario administrador principal." });

                if (!await _roleManager.RoleExistsAsync(dto.Rol))
                    return BadRequest(new { mensaje = $"El rol '{dto.Rol}' no existe." });

                var rolesActuales = await _userManager.GetRolesAsync(user);

                if (rolesActuales.Contains(dto.Rol))
                {
                    return BadRequest(new { mensaje = $"El usuario ya tiene el rol '{dto.Rol}'." });
                }

                if (rolesActuales.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesActuales);

                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(new
                        {
                            mensaje = "No se pudieron remover los roles actuales del usuario.",
                            errores = removeResult.Errors.Select(e => e.Description)
                        });
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, dto.Rol);

                if (!addResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo asignar el nuevo rol al usuario.",
                        errores = addResult.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Rol actualizado correctamente.",
                    userName = user.UserName,
                    rol = dto.Rol
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el rol del usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al cambiar el rol." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Actualizar)]
        [HttpPut("{id}/password")]
        public async Task<IActionResult> CambiarPassword(string id, [FromBody] CambiarPasswordUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NuevaPassword))
                    return BadRequest(new { mensaje = "La nueva contraseña es obligatoria." });

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.NuevaPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo cambiar la contraseña del usuario.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Contraseña actualizada correctamente.",
                    userName = user.UserName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar la contraseña del usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al cambiar la contraseña." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Activar)]
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> ActivarUsuario(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                if (user.Activo == true)
                    return BadRequest(new { mensaje = "Usuario ya activo." });

                user.Activo = true;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo activar el usuario.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Usuario activado correctamente.",
                    userName = user.UserName,
                    activo = user.Activo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al activar usuario." });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Desactivar)]
        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> DesactivarUsuario(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                var rolesActuales = await _userManager.GetRolesAsync(user);
                if (rolesActuales.Contains("Administrador"))
                    return BadRequest(new { mensaje = "No se puede desactivar el usuario administrador principal." });

                //if (user.UserName == "admin")
                //    return BadRequest(new { mensaje = "No se puede desactivar el usuario administrador principal." });

                if (user.Activo == false)
                    return BadRequest(new { mensaje = "Usuario ya desactivado." });


                user.Activo = false;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo desactivar el usuario.",
                        errores = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new
                {
                    mensaje = "Usuario desactivado correctamente.",
                    userName = user.UserName,
                    activo = user.Activo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuario {UserId}", id);
                return StatusCode(500, new { mensaje = "Error interno al desactivar usuario." });
            }
        }
    }
}
