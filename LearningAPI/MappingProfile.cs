using AutoMapper;
using Uplay.Models;

namespace Uplay
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reward, RewardDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
        }
    }
}
