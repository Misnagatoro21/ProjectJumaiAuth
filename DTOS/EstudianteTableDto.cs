namespace ProjectJumaiAuth.DTOS
{
    public class EstudianteTableDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CursoId { get; set; }
        public string CursoNombre { get; set; } = string.Empty;

    }
}
