using ProjectJumaiAuth.Models.ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = [];
        public ICollection<RolePermiso> RolePermisos { get; set; } = [];
    }
}