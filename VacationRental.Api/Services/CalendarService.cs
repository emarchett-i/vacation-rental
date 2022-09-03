using System;
using System.Collections.Generic;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public class CalendarService
    {
        private readonly RentalRepository _rentalRepository;
        private readonly BookingRepository _bookingRepository;

        public CalendarService(RentalRepository rentalRepository,
                               BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
        }

        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            IEnumerable<Booking> bookings = _bookingRepository.GetAll();

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                foreach (var booking in bookings)
                {
                    if (booking.RentalId == rentalId
                        && booking.StartDateTime <= date.Date && booking.StartDateTime.AddDays(booking.NumberOfNights) > date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id });
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
