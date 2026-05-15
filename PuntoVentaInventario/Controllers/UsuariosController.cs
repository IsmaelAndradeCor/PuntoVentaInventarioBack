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

        [HttpPut("activar/{userName}")]
        public async Task<IActionResult> ActivarUsuario(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName.Trim());

                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

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
