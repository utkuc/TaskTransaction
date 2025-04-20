using AutoMapper;
using TaskTransaction.Models.User.Dto;

namespace TaskTransaction.Models.User;

public class UserMapProfile: Profile
{
    public UserMapProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<DeleteUserDto, User>();
    }
}