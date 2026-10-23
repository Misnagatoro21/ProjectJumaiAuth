using System.ComponentModel.DataAnnotations;

namespace ProjectJumaiAuth.DTOS
{
    public class SaveestudianteDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int CursoId { get; set; }
    }
}
