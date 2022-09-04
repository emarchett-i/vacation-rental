using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace VacationRental.Api.Mapper
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BookingProfile());
                mc.AddProfile(new RentalProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
