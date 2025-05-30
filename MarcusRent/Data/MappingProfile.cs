using AutoMapper;
using MarcusRent.Classes;
using MarcusRent.Models;
using AutoMapper;

namespace MarcusRent.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Car, CarViewModel>();
            CreateMap<CarViewModel, Car>();
            CreateMap<Car, CarViewModel>().ReverseMap();
            CreateMap<Car, CarViewModel>()
    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.CarImages.Select(ci => ci.Url)));

        }


    }
}
