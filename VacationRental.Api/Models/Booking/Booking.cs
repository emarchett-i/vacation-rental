using System;

namespace VacationRental.Api.Models.Booking
{
    public class Booking
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public int NumberOfNights { get; set; }
        public DateTime EndDate 
        {
            get
            {
                return StartDate.Date.AddDays(NumberOfNights - 1);
            }
        }
        public int Unit { get; set; }
    }
}
