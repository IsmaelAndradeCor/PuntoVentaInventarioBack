using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
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

        [HttpPost("crear_usuario")]
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

        [HttpPut("cambiar_nombre_completo/{userName}")]
        public async Task<IActionResult> CambiarNombreCompleto(string userName, [FromBody] CambiarNombreCompletoUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                    return BadRequest(new { mensaje = "El nombre completo es obligatorio." });

                var user = await _userManager.FindByNameAsync(userName.Trim());

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

        [HttpPut("cambiar_rol/{userName}")]
        public async Task<IActionResult> CambiarRol(string userName, [FromBody] CambiarRolUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Rol))
                    return BadRequest(new { mensaje = "El rol es obligatorio." });

                if (dto.Rol != "Empleado")
                    return BadRequest(new { mensaje = "Solo se permite asignar el rol de Empleado." });

                var user = await _userManager.FindByNameAsync(userName.Trim());

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

        [HttpPut("cambiar_password/{userName}")]
        public async Task<IActionResult> CambiarPassword(string userName, [FromBody] CambiarPasswordUpsertDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NuevaPassword))
                    return BadRequest(new { mensaje = "La nueva contraseña es obligatoria." });

                var user = await _userManager.FindByNameAsync(userName.Trim());

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

        [HttpPut("activar/{userName}")]
        public async Task<IActionResult> ActivarUsuario(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName.Trim());

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

        [HttpPut("desactivar/{userName}")]
        public async Task<IActionResult> DesactivarUsuario(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName.Trim());

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                if (user.UserName == "admin")
                    return BadRequest(new { mensaje = "No se puede desactivar el usuario administrador principal." });

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
