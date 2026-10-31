namespace ProjectJumaiAuth.DTOS
{
    public class UserPermissionOverrideDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
