using System.ComponentModel.DataAnnotations;

namespace ProjectJumaiAuth.DTOS
{
    public class SaveCursoDto
    {

        public int? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength]
        public string? Descripcion { get; set; }
    }
}
