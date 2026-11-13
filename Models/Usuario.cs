namespace ProjectJumaiAuth.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    
        public DateTime? RefreshTokenExpiry { get; set; }

        public int RolId { get; set; }
        public Role Rol { get; set; } = null!;

        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];
        public ICollection<Tarea> Tareas { get; set; } = [];
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiryTime { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}