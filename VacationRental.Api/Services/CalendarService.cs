using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Calendar;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public class CalendarService
    {
        private readonly RentalRepository _rentalRepository;
        private readonly BookingRepository _bookingRepository;
        private readonly ILogger<CalendarService> _logger;

        public CalendarService(RentalRepository rentalRepository,
                               BookingRepository bookingRepository,
                               ILogger<CalendarService> logger)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets the calendar with all the bookings for the specified rental and date range.
        /// </summary>
        /// <param name="rentalId">The Rental Id.</param>
        /// <param name="start">The time range start date.</param>
        /// <param name="nights">The number of nights.</param>
        /// <returns></returns>
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            try
            {
                var result = new CalendarViewModel
                {
                    RentalId = rentalId,
                    Dates = new List<CalendarDateViewModel>()
                };

                Rental rental = _rentalRepository.Get(rentalId);

                SetBookings(result, rental, start, nights);

                SetPreparationTimes(result, rental);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to get the Calendar");

                throw ex;
            }
        }

        private void SetBookings(CalendarViewModel calendar, Rental rental, DateTime start, int nights)
        {
            IEnumerable<Booking> bookings = _bookingRepository.GetAll();

            for (var i = 0; i < nights; i++)
            {
                var date = start.Date.AddDays(i);

                var calendarDate = new CalendarDateViewModel
                {
                    Date = date,
                    Bookings = bookings.Where(booking => booking.RentalId == rental.Id &&
                                                         booking.StartDate <= date &&
                                                         booking.EndDate >= date)
                                                  .Select(x => new CalendarBookingViewModel { Id = x.Id, Unit = x.Unit })
                                                  .ToList()
                };

                calendar.Dates.Add(calendarDate);
            }
        }

        private void SetPreparationTimes(CalendarViewModel calendar, Rental rental)
        {
            Dictionary<DateTime, CalendarDateViewModel> calendarDates = calendar.Dates.ToDictionary(k => k.Date, v => v);

            foreach (var calendarDate in calendarDates)
            {
                IEnumerable<int> unitsBooked = calendarDate.Value.Bookings.Select(x => x.Unit);

                foreach (var unitBooked in unitsBooked)
                {
                    if (calendarDates.ContainsKey(calendarDate.Key.AddDays(1)) && 
                        !calendarDates[calendarDate.Key.AddDays(1)].Bookings.Any(x => x.Unit == unitBooked))
                    {
                        TryAddPreparationTime(calendarDates, calendarDate.Key, unitBooked, rental.PreparationTimeInDays);
                    }
                }
            }
            calendar.Dates = calendarDates.Values.ToList();
        }

        private void TryAddPreparationTime(Dictionary<DateTime, CalendarDateViewModel> calendarDates, DateTime currentDate, int unit, int preparationTimeInDays)
        {
            for (int i = 1; i <= preparationTimeInDays; i++)
            {
                if (calendarDates.ContainsKey(currentDate.AddDays(i)) && !calendarDates[currentDate.AddDays(i)].Bookings.Any(x => x.Unit == unit))
                {
                    calendarDates[currentDate.AddDays(i)].PreparationTimes.Add(new CalendarPreparationTimeViewModel(unit));
                }
            }
        }
    }
}
