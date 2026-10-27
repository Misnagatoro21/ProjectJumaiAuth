using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Models
{
    public class UsuarioPermiso
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int PermisoId { get; set; }
        public Permiso Permiso { get; set; } = null!;

        public bool IsGranted { get; set; }
    }
}