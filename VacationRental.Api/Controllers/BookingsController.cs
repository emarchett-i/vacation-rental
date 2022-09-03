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
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly BookingService _bookingService;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings,
            BookingService bookingService)
        {
            _rentals = rentals;
            _bookings = bookings;
            _bookingService = bookingService;

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
            if (!_rentals.ContainsKey(model.RentalId))
                throw new ApplicationException("Rental not found");

            ResourceIdViewModel key = _bookingService.Create(model);

            return key;
        }
    }
}
