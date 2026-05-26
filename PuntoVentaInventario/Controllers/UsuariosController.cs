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
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Policy = Permissions.Usuarios.ActivosVer)]
        [HttpGet("activos")]
        public async Task<ActionResult<List<UsuarioPermisosResponseDto>>> ObtenerUsuariosActivos()
        {
            try
            {
                var usuarios = await _userManager.Users
                    .Where(u => u.Activo)
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

        [Authorize(Policy = Permissions.Usuarios.InactivosVer)]
        [HttpGet("inactivos")]
        public async Task<ActionResult<List<UsuarioPermisosResponseDto>>> ObtenerUsuariosInactivos()
        {
            try
            {
                var usuarios = await _userManager.Users
                    .Where(u => u.Activo!)
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
                return StatusCode(500, new
                {
                    mensaje = $"Error al obtener el usuario: {ex.Message}"
                });
            }
        }

        [Authorize(Policy = Permissions.Usuarios.Crear)]
        [HttpPost]
        public async Task<IActionResult> Crearsuario([FromBody] CrearUsuarioUpsertDto dto)
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
                        mensaje = "No se pudo crear el usuario.",
                        errores = result.Errors.Select(e => e.Description)
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

                return Ok(new
                {
                    mensaje = "Usuario creado correctamente.",
                    userName = nuevoUsuario.UserName,
                    nombreCompleto = nuevoUsuario.NombreCompleto,
                    rol = dto.Rol
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = $"Error al crear el usuario: {ex.Message}"
                });
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
                return StatusCode(500, new
                {
                    mensaje = $"Error al actualizar el nombre completo: {ex.Message}"
                });
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
                return StatusCode(500, new
                {
                    mensaje = $"Error al cambiar el rol: {ex.Message}"
                });
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
                return StatusCode(500, new
                {
                    mensaje = $"Error al cambiar la contraseña: {ex.Message}"
                });
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
                return StatusCode(500, new { mensaje = $"Error al activar usuario: {ex.Message}" });
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
                return StatusCode(500, new { mensaje = $"Error al desactivar usuario: {ex.Message}" });
            }
        }
    }
}
