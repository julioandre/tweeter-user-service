using AutoMapper;
using user_service.Models;

namespace user_service.Configurations;

public class MapperInitializer:Profile
{
    public MapperInitializer()
    {
        CreateMap<UserEntity, UserDTO>().ReverseMap();
    }
    
}