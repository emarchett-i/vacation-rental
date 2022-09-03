using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Rental;

namespace VacationRental.Api.Mapper
{
    public class RentalProfile : Profile
    {
        public RentalProfile()
        {
            CreateMap<Rental, RentalViewModel>();

            CreateMap<RentalBindingModel, Rental>();
        }
    }
}
