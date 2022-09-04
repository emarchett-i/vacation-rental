using System;
using System.Collections.Generic;
using VacationRental.Api.Models.Calendar;

namespace VacationRental.Api.Models
{
    public class CalendarDateViewModel
    {
        public CalendarDateViewModel()
        {
            Bookings = new List<CalendarBookingViewModel>();
            PreparationTimes = new List<CalendarPreparationTimeViewModel>();
        }

        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }
        public List<CalendarPreparationTimeViewModel> PreparationTimes { get; set; }
    }
}
