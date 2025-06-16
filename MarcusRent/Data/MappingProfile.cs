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


            CreateMap<CarViewModel, Car>()
                .ForMember(dest => dest.CarImages, 
                opt => opt.MapFrom(src => src.ImageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => new CarImage { Url = url })));


     


            // Order -> OrderViewModel
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Cars, opt => opt.Ignore()) // sätts manuellt i controller
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Car.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Car.Year))
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.Car.PricePerDay));

            // OrderViewModel -> Order
            CreateMap<OrderViewModel, Order>()
                .ForMember(dest => dest.Car, opt => opt.Ignore())        // viktigt!
                .ForMember(dest => dest.Customer, opt => opt.Ignore());  // viktigt!
        }
    }
}
