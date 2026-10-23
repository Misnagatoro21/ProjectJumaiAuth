namespace ProjectJumaiAuth.DTOS
{
    public class UserPermissionDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<string> EffectivePermission { get; set; } = [];
        public List<UserPermissionOverride> DirectPermission { get; set; } = [];
    
}
}
