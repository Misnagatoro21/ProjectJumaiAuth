namespace ProjectJumaiAuth.Models
{
    public class RolePermiso
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
        public int PermisoId { get; set; }

        public virtual Permiso Permiso { get; set; } = null!;
    }
}
