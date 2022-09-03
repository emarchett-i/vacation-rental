using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarService _calendarService;
        private readonly RentalService _rentalService;

        public CalendarController(CalendarService calendarService,
                                  RentalService rentalService)
        {
            _calendarService = calendarService;
            _rentalService = rentalService;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");

            if (!_rentalService.Exists(rentalId))
                throw new ApplicationException("Rental not found");

            return _calendarService.Get(rentalId, start, nights);
        }
    }
}
