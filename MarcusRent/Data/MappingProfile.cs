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
  

            CreateMap<Car, CarViewModel>()
    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.CarImages.Select(ci => ci.Url)))
    .ForMember(dest => dest.CarDescription, opt => opt.MapFrom(src => src.CarDescription)) // <-- Lägg till detta
    .ForMember(dest => dest.TotalEarnings, opt => opt.Ignore())
    .ForMember(dest => dest.CurrentRentalEndDate, opt => opt.Ignore())
    .ForMember(dest => dest.CurrentCustomerName, opt => opt.Ignore());

            CreateMap<CarViewModel, Car>()
    .ForMember(dest => dest.CarImages,
        opt => opt.MapFrom(src => src.ImageUrls
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => new CarImage { Url = url })))
    .ForMember(dest => dest.CarDescription, opt => opt.MapFrom(src => src.CarDescription)); // <-- Lägg till detta





            // Order -> OrderViewModel -> Order
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Cars, opt => opt.Ignore()) // Sätts manuellt i controller
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Car.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Car.Year))
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.Car.PricePerDay))
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => $"{src.Car.Brand} {src.Car.Model}"))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                 .ForMember(dest => dest.CarDescription, opt => opt.MapFrom(src => src.Car.CarDescription))
                //.ForMember(dest => dest.ActiveOrder, opt => opt.MapFrom(src => src.EndDate > DateTime.Now))
                .ReverseMap();

            


            // ApplicationUser -> AdminCustomerViewModel
            CreateMap<ApplicationUser, CustomerViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserName)); // Ändra om du har ett FullName-fält


            CreateMap<ApplicationUser, OrderViewModel>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<ApplicationUser, CustomerViewModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.ApprovedByAdmin, opt => opt.MapFrom(src => src.ApprovedByAdmin));






            CreateMap<OrderViewModel, Order>()
             .ForMember(dest => dest.Car, opt => opt.Ignore())
             .ForMember(dest => dest.Customer, opt => opt.Ignore())
             .ForMember(dest => dest.UserId, opt => opt.Ignore())
             .ForMember(dest => dest.Price, opt => opt.Ignore());

        }
    }
}
