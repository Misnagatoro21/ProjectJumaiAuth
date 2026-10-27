using AutoMapper;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDto>()
                .ForMember(d => d.AccessToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.ExpiresAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshTokenExpiry, opt => opt.Ignore())
                .ForMember(d => d.Rol, opt => opt.Ignore())
                .ForMember(d => d.RolId, opt => opt.Ignore());
        }
    }
}