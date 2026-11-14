using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Usuarios.AnyAsync()) return;

            // Evita duplicar roles si ya existen (por ejemplo, del script SQL)
            var rolAdmin = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Admin");
            if (rolAdmin == null)
            {
                rolAdmin = new Role { Nombre = "Admin" };
                context.Roles.Add(rolAdmin);
            }

            var rolUsuario = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Usuario");
            if (rolUsuario == null)
            {
                rolUsuario = new Role { Nombre = "Usuario" };
                context.Roles.Add(rolUsuario);
            }
            await context.SaveChangesAsync();

            var admin = new Usuario
            {
                NombreUsuario = "admin",
                Email = "admin@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Admin123!"),
                RolId = rolAdmin.Id,
                EmailConfirmed = true
            };
            var usuario = new Usuario
            {
                NombreUsuario = "usuario",
                Email = "usuario@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Usuario123!"),
                RolId = rolUsuario.Id,
                EmailConfirmed = true
            };

            context.Usuarios.AddRange(admin, usuario);
            await context.SaveChangesAsync();
        }
    }
}