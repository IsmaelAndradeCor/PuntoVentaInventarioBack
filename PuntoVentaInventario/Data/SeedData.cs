using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Models.Entities;
using System.Security.Claims;

namespace PuntoVentaInventario.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string[] roles = { "Administrador", "Empleado" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUserName = configuration["Seed:AdminUserName"] ?? "admin";
            var adminPassword = configuration["Seed:AdminPassword"] ?? "Admin123*";
            var empleadoUserName = configuration["Seed:EmpleadoUserName"] ?? "empleado";
            var empleadoPassword = configuration["Seed:EmpleadoPassword"] ?? "Empleado123*";

            var admin = await userManager.FindByNameAsync(adminUserName);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminUserName,
                    NombreCompleto = "Administrador General",
                    Activo = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(admin, "Administrador");
                await EnsurePermissionsAsync(userManager, admin, Permissions.All);
            }

            var empleado = await userManager.FindByNameAsync(empleadoUserName);

            if (empleado == null)
            {
                empleado = new ApplicationUser
                {
                    UserName = empleadoUserName,
                    NombreCompleto = "Gilberto Andrade",
                    Activo = true
                };

                var result = await userManager.CreateAsync(empleado, empleadoPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(empleado, "Empleado");
                await EnsurePermissionsAsync(userManager, empleado, new[]
                {
                    Permissions.Home.Ver,
                    Permissions.Ventas.Realizar
                });
            }
        }

        private static async Task EnsurePermissionsAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            IEnumerable<string> permissions)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);

            var missingClaims = permissions
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .Where(p => !existingClaims.Any(c => c.Type == "permission" && c.Value == p))
                .Select(p => new Claim("permission", p))
                .ToList();

            if (missingClaims.Count == 0)
                return;

            var result = await userManager.AddClaimsAsync(user, missingClaims);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}