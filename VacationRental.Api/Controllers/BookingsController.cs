using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly RentalService _rentalService;

        public BookingsController(
            BookingService bookingService,
            RentalService rentalService)
        {
            _bookingService = bookingService;
            _rentalService = rentalService;

        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            if (bookingId == 0)
                throw new ApplicationException("Booking Id can't be 0");

            return _bookingService.Get(bookingId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentalService.Exists(model.RentalId))
                throw new ApplicationException("Rental not found");

            ResourceIdViewModel key = _bookingService.Create(model);

            return key;
        }
    }
}
