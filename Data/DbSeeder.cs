using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Models;


namespace ProjectJumaiAuth.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Usuarios.AnyAsync()) return;

            var rolAdmin = new Role { Nombre = "Admin" };
            var rolUsuario = new Role { Nombre = "Usuario" };
            context.Roles.AddRange(rolAdmin, rolUsuario);
            await context.SaveChangesAsync();

            var admin = new Usuario
            {
                NombreUsuario = "admin",
                Email = "admin@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Admin123!"),
                RolId = rolAdmin.Id
            };
            var usuario = new Usuario
            {
                NombreUsuario = "usuario",
                Email = "usuario@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Usuario123!"),
                RolId = rolUsuario.Id
            };
            
        }
    }
}