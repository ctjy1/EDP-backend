using AutoMapper;
using LearningAPI.Models;
using UPlay.Models;

namespace UPlay
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gallery, GalleryDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<User, UserDTO>();
        }

    }
}
