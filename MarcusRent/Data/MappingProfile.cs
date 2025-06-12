using AutoMapper;
using MarcusRent.Classes;
using MarcusRent.Models;
using System.Linq;

namespace MarcusRent.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Car <-> CarViewModel
            CreateMap<Car, CarViewModel>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.CarImages.Select(ci => ci.Url)))
                .ReverseMap();

            // Order -> OrderViewModel
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Cars, opt => opt.Ignore()); // sätts manuellt i controllern

            // OrderViewModel -> Order
            CreateMap<OrderViewModel, Order>()
                .ForMember(dest => dest.Car, opt => opt.Ignore())        // viktigt!
                .ForMember(dest => dest.Customer, opt => opt.Ignore());  // viktigt!
        }
    }
}
