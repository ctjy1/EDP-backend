using AutoMapper;
using Uplay.Models;
using Uplay.Models;

namespace Uplay
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
