using AutoMapper;
using LearningAPI.Models;

namespace LearningAPI
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
