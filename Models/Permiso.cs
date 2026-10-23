namespace ProjectJumaiAuth.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];
        public ICollection<RolePermiso> RolePermisos { get; set; } = [];

    }
}
