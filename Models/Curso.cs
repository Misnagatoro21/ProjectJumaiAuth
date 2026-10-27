namespace ProjectJumaiAuth.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    }
}