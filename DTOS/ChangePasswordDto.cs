namespace ProjectJumaiAuth.DTOS
{
    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    
}
}
