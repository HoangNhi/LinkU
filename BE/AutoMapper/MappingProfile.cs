using AutoMapper;
using ENTITIES.DbContent;
using MODELS.OTP.Dtos;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // User
            CreateMap<User, MODELUser>().ReverseMap();
            CreateMap<User, PostUserRequest>().ReverseMap();
            CreateMap<User, RegisterRequest>().ReverseMap();

            // OTP
            CreateMap<OTP, MODELOTP>().ReverseMap();

            // RefreshToken
            CreateMap<RefreshToken, MODELRefreshToken>().ReverseMap();
        }
    }
}
