using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Models.ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Usuarios.AnyAsync())
                return;

            var admin = new Usuario
            {
                NombreUsuario = "admin",
                Email = "admin@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = "Admin"
            };
            var usuario = new Usuario
            {
                NombreUsuario = "usuario",
                Email = "usuario@correo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Usuario123!"),
                Rol = "Usuario"
            };

            context.Usuarios.AddRange(admin, usuario);
            await context.SaveChangesAsync();

            context.Tareas.AddRange(
                new Tarea { Titulo = "Probar el login", UsuarioId = usuario.Id },
                new Tarea { Titulo = "Revisar el token JWT", UsuarioId = admin.Id });
            await context.SaveChangesAsync();
        }
    }
}