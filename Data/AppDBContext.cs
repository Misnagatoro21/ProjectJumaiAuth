using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Tarea> Tareas => Set<Tarea>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<RolePermiso> RolePermissions => Set<RolePermiso>();
        public DbSet<UsuarioPermiso> UsuarioPermisos => Set<UsuarioPermiso>();
        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RolePermiso>()
                .HasKey(rp => new { rp.RoleId, rp.PermisoId });

            modelBuilder.Entity<UsuarioPermiso>()
                .HasKey(up => new { up.UsuarioId, up.PermisoId });

            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Curso)
                .WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}