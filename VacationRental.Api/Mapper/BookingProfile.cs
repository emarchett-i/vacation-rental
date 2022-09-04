using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;

namespace VacationRental.Api.Mapper
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingViewModel>()
                .ForMember(dest => dest.Start, src => src.MapFrom(x => x.StartDate))
                .ForMember(dest => dest.Nights, src => src.MapFrom(x => x.NumberOfNights));

            CreateMap<BookingBindingModel, Booking>()
                .ForMember(dest => dest.StartDate, src => src.MapFrom(x => x.Start))
                .ForMember(dest => dest.NumberOfNights, src => src.MapFrom(x => x.Nights));
        }
    }
}
