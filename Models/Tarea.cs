using ProjectJumaiAuth.Models.ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool Completada { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}