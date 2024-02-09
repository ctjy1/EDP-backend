using AutoMapper;
using Uplay.Models;
using Uplay.Models.RewardModels;

namespace Uplay
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gallery, GalleryDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<Reward, RewardDTO>();
        }

    }
}
