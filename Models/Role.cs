namespace ProjectJumaiAuth.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public ICollection<Usuario> Usuario { get; set; } = [];
        public ICollection<RolePermiso> RolePermisos { get; set; } = [];
    }
}
