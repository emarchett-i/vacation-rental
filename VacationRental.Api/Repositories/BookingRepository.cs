using System;
using System.Collections.Generic;
using VacationRental.Api.Models.Booking;

namespace VacationRental.Api.Repositories
{
    public class BookingRepository
    {
        private readonly IDictionary<int, Booking> _bookings;

        public BookingRepository(IDictionary<int, Booking> bookings)
        {
            _bookings = bookings;
        }

        public Booking Get(int id)
        {
            if (_bookings.ContainsKey(id))
                return _bookings[id];

            return default(Booking);
        }

        public IEnumerable<Booking> GetAll()
        {
            return _bookings.Values;
        }

        public int Add(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException("booking");

            booking.Id = GenerateNewId();

            _bookings.Add(booking.Id, booking);

            return booking.Id;
        }

        private int GenerateNewId()
        {
            return _bookings.Count + 1;
        }
    }
}
