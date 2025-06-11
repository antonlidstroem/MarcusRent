using AutoMapper;
using MarcusRent.Classes;
using MarcusRent.Models;
using System.Collections.Generic;
using System.Linq;

namespace MarcusRent.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mappar Car <-> CarViewModel (tvåvägs)
            CreateMap<Car, CarViewModel>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.CarImages.Select(ci => ci.Url)))
                .ReverseMap();

            // Mappar Order -> OrderViewModel
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.CarId,
                    opt => opt.MapFrom(src =>
                        src.CarOrders != null && src.CarOrders.Any()
                        ? src.CarOrders.First().CarId
                        : 0))  // Null-säkerhet för CarOrders
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Id : null)) // Null-säkerhet Customer
                .ForMember(dest => dest.CarOptions, opt => opt.Ignore()) // Sätts i controller
                .ForMember(dest => dest.UserOptions, opt => opt.Ignore());

            // Mappar OrderViewModel -> Order
            CreateMap<OrderViewModel, Order>()
                .ForMember(dest => dest.CarOrders,
                    opt => opt.MapFrom(src =>
                        src.CarId > 0
                        ? new List<CarOrder> { new CarOrder { CarId = src.CarId } }
                        : new List<CarOrder>())) // Skapar CarOrder-lista, eller tom om inget valt
                .ForMember(dest => dest.Customer, opt => opt.Ignore()); // Sätts i controller
        }
    }
}
