namespace ProjectJumaiAuth.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;
    }
}