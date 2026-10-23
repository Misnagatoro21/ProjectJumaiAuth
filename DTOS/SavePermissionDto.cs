using System.ComponentModel.DataAnnotations;

namespace ProjectJumaiAuth.DTOS
{
    public class SavePermissionDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-z][a-z0-9._]*$", ErrorMessage = "Use formato: recurso.accion (ej: estudiante.crear)")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
