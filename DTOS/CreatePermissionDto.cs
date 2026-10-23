using System.ComponentModel.DataAnnotations;

namespace ProjectJumaiAuth.DTOS
{
    public class CreatePermissionDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(150)]
        [RegularExpression(@"[a-z][a-z0-9._]*$", ErrorMessage = "Use el formato recurso.Requerido")]
        public required string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public required string Description { get; set; }
    }
}
