using AutoMapper;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Mapping
{
    public class UserProfile
    {
        public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDto>()
                .ForMember(d => d.AccessToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.ExpiresAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(d => d.Password, opt => opt.Ignore())
                .ForMember(d => d.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(d => d.EmailConfirmToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(d => d.PasswordResetToken, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore());
        }
    }
}
