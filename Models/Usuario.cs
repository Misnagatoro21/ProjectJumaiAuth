namespace ProjectJumaiAuth.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public int RolId { get; set; }
        public Role Rol { get; set; } = null!;

        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];
        public ICollection<Tarea> Tareas { get; set; } = [];
    }
}