using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using VacationRental.Api.Mapper;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;

namespace VacationRental.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacation rental information", Version = "v1" }));

            services.AddSingleton<IDictionary<int, Rental>>(new Dictionary<int, Rental>());
            services.AddSingleton<IDictionary<int, Booking>>(new Dictionary<int, Booking>());

            services.AddScoped<BookingService, BookingService>();
            services.AddScoped<CalendarService, CalendarService>();
            services.AddScoped<RentalService, RentalService>();

            services.AddScoped<BookingRepository, BookingRepository>();
            services.AddScoped<RentalRepository, RentalRepository>();

            // Auto Mapper Configurations
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
        }
    }
}
