using System;

namespace VacationRental.Api.Models.Booking
{
    public class Booking
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int NumberOfNights { get; set; }
    }
}
